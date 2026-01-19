using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models;
using LearnCSharp.Application.Models.DTOs.Product;
using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;
using System.Linq.Expressions;

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
                if (model.Thumbnail != null)
                {
                    imagePath = await _imageService.UploadImageAsync(model.Thumbnail);
                }
                var data = new Product()
                {
                    Name = model.Name,
                    Price = model.Price,
                    Thumbnail = imagePath,
                    Description = model.Description,
                    CategoryId = model.CategoryId,
                    CreatedDate = DateTime.UtcNow,
                };
                await _unitOfWork.Product.CreateAsync(data);
                await _unitOfWork.CompleteAsync();

                if (model.Images?.Any() == true)
                {
                    var imagePaths = await _imageService.UploadMultipleImageAsync(model.Images);
                    var dataProductImage = imagePaths.Select(a => new ProductImage()
                    {
                        ProductId = data.Id,
                        ImageUrl = a,
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

        public async Task DeleteAsync(int id)
        {
            var product = await _unitOfWork.Product.GetByfilterAsync(a => a.Id == id);
            try
            {
                await _unitOfWork.Product.RemoveAsync(product);
                await _unitOfWork.CompleteAsync();
            }
            catch
            {
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
            var (products, totalCount) = await _unitOfWork.Product.GetPagedAsync(filter, ((pageIndex - 1) * pageSize), pageSize,false,includes:a=>a.Category);
            var data = products.Select(a => new ProductDTO
            {
                Id = a.Id,
                Name = a.Name,
                Price = a.Price,
                Thumbnail = a.Thumbnail,
                Description = a.Description,
                CategoryId = a.CategoryId,
                CategoryName=a.Category.Name,
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
            var product = await _unitOfWork.Product.GetByfilterAsync(a => a.Id == id);

            var dataProductImage = await _productImageService.GetListProductImageByIdAsync(id);
            var data = new ProductDTO()
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Thumbnail = product.Thumbnail,
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
                var product = await _unitOfWork.Product.GetByfilterAsync(a => a.Id == id);
                if (product == null)
                {
                    throw new InvalidOperationException($"Product with ID {id} not found.");
                }
                product.Name = model.Name ?? product.Name;
                product.Price = model.Price > 0 ? model.Price.Value : product.Price;
                product.Description = model.Description ?? product.Description;
                product.CategoryId = model.CategoryId ?? product.CategoryId;
                product.UpdatedDate = DateTime.UtcNow;
                if (model.Thumbnaill != null)
                {
                    if (!string.IsNullOrEmpty(product.Thumbnail))
                    {
                        _imageService.DeleteImage(product.Thumbnail);
                    }
                    product.Thumbnail = await _imageService.UploadImageAsync(model.Thumbnaill);
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