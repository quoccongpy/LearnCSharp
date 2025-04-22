using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models;
using LearnCSharp.Application.Models.DTOs.Product;
using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LearnCSharp.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;
        private readonly IProductImageService _productImageService;

        public ProductService(IUnitOfWork unitOfWork, IImageService imageService, IProductImageService productImageService)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
            _productImageService = productImageService;
        }

        public async Task CreateAsync(ProductCreateDTO model)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                string imagePath = null;
                if (model.Thumbnaill != null)
                {
                    imagePath = await _imageService.UploadImageAsync(model.Thumbnaill);
                }
                var data = new Product()
                {
                    Name = model.Name,
                    Price = model.Price,
                    Thumbnaill = imagePath,
                    Description = model.Description,
                    CategoryId = model.CategoryId,
                    CreatedDate = DateTime.Now,
                };
                await _unitOfWork.Product.CreateAsync(data);
                await _unitOfWork.CompleteAsync();

                if (model.Image != null && model.Image.Count > 0)
                {
                    var imagePaths = await _imageService.UploadMultipleImageAsync(model.Image);
                    var dataProductImage = imagePath.Select(a => new ProductImage()
                    {
                        ProductId = data.Id,
                        ImageUrl = imagePath,
                    }).ToList();
                    foreach (var item in dataProductImage)
                    {
                        await _unitOfWork.ProductImage.CreateAsync(item);
                    }
                    await _unitOfWork.CompleteAsync();
                }
                await _unitOfWork.CommitTransactionAsync();
            }
            catch 
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<PagedResult<ProductDTO>> GetAllProductPagingAsync(string? keyword, int pageIndex = 1, int pageSize = 10)
        {
            Expression<Func<Product, bool>> filter = null;
            if (!string.IsNullOrEmpty(keyword))
            {
                filter = a => a.Name.Contains(keyword);
            }
            var (products, totalCount) = await _unitOfWork.Product.GetPagedAsync(filter, ((pageIndex - 1) * pageSize), pageSize);
            var data = products.Select(a => new ProductDTO
            {
                Id = a.Id,
                Name = a.Name,
                Price = a.Price,
                Thumbnaill = a.Thumbnaill,
                Description = a.Description,
                CategoryId = a.CategoryId,
            }).ToList();
            var result = new PagedResult<ProductDTO>
            {
                Results = data,
                CurrentPage = pageIndex,
                RowCount = totalCount,
                PageSize = pageSize
            };
            return result;
        }

        public async Task<ProductDTO> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Product.GetByIdAsync(a => a.Id == id);

            var dataProductImage = await _productImageService.GetListProductImageByIdAsync(id);
            var data = new ProductDTO()
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Thumbnaill = product.Thumbnaill,
                Description = product.Description,
                CategoryId = product.CategoryId,
                ProductImagesList = dataProductImage.ToList(),
            };
            return data;
        }

        public async Task Update(int id, ProductUpdateDTO model)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var product = await _unitOfWork.Product.GetByIdAsync(a => a.Id == id);
                if (product == null)
                {
                    throw new InvalidOperationException($"Product with ID {id} not found.");
                }
                product.Name = model.Name ?? product.Name;
                product.Price = model.Price > 0 ? model.Price.Value : product.Price;
                product.Description = model.Description ?? product.Description;
                product.CategoryId = model.CategoryId ?? product.CategoryId;
                product.UpdatedDate = DateTime.Now;
                if (model.Thumbnaill != null)
                {
                    if (!string.IsNullOrEmpty(product.Thumbnaill))
                    {
                        _imageService.DeleteImage(product.Thumbnaill);
                    }
                    product.Thumbnaill = await _imageService.UploadImageAsync(model.Thumbnaill);
                }
                _unitOfWork.Product.Update(product);
                await _unitOfWork.CompleteAsync();

                if (model.Image != null && model.Image.Count > 0 || model.ListRetainIdsImage != null)
                {
                    var currentImages = await _productImageService.GetListProductImageByIdAsync(id);
                    var retainIdsImage = model.ListRetainIdsImage ?? new List<int>();
                    foreach (var image in currentImages)
                    {
                        if (!retainIdsImage.Contains(image.Id))
                        {
                            _imageService.DeleteImage(image.ImageUrl);
                            await _productImageService.DeleteAsync(image.Id);
                        }
                    }
                    if (model.Image != null && model.Image.Count > 0)
                    {
                        var uploadedPaths = new List<string>();
                        try
                        {
                            uploadedPaths = await _imageService.UploadMultipleImageAsync(model.Image);
                            foreach (var path in uploadedPaths)
                            {
                                var dataProductImage = new ProductImage()
                                {
                                    ProductId = id,
                                    ImageUrl = path,
                                };
                                await _unitOfWork.ProductImage.CreateAsync(dataProductImage);
                            }
                        }
                        catch (Exception ex)
                        {
                            _imageService.DeleteMultipleImage(uploadedPaths);
                            throw new Exception($"Error uploading new images: {ex.Message}");
                        }
                    }
                }
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}