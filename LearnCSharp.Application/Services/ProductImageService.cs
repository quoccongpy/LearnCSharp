using LearnCSharp.Application.Interfaces;
using LearnCSharp.Application.Models.DTOs.ProductImage;
using LearnCSharp.Domain.Interfaces;

namespace LearnCSharp.Application.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductImageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _unitOfWork.ProductImage.GetByIdAsync(a => a.ProductId == id);
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.ProductImage.RemoveAsync(product);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<IEnumerable<ProductImageDTO>> GetListProductImageByIdAsync(int id)
        {
            var productImage = await _unitOfWork.ProductImage.GetAllAsync(a => a.ProductId == id);
            var data = productImage.Select(a => new ProductImageDTO()
            {
                ImageUrl = a.ImageUrl,
                ProductId = a.ProductId,
            }).ToList();
            return data;
        }
    }
}