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
            var productImage = await _unitOfWork.ProductImage.GetByfilterAsync(a => a.Id == id);
            await _unitOfWork.ProductImage.RemoveAsync(productImage);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<ProductImageDTO>> GetListProductImageByIdAsync(int id)
        {
            var productImage = await _unitOfWork.ProductImage.GetAllAsync(a => a.ProductId == id);
            var data = productImage.Select(a => new ProductImageDTO()
            {
                Id=a.Id,
                ImageUrl = a.ImageUrl,
                ProductId = a.ProductId,
            }).ToList();
            return data;
        }
    }
}