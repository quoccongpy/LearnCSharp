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

        public ProductService(IUnitOfWork unitOfWork, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
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
                    CreatedDate=DateTime.Now,
                };
                await _unitOfWork.Product.CreateAsync(data);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception(ex.Message);
            }
        }

        public async Task<PagedResult<ProductDTO>> GetAllProductPagingAsync(string? keyword, int pageIndex=1 , int pageSize = 10)
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
            var data = new ProductDTO()
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Thumbnaill = product.Thumbnaill,
                Description = product.Description,
                CategoryId = product.CategoryId,
            };
            return data;
        }

        public async Task Update(int id, ProductUpdateDTO model)
        {
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
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                _unitOfWork.Product.Update(product);
                await _unitOfWork.CompleteAsync();
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