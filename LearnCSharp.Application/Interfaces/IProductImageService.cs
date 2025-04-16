using LearnCSharp.Application.Models.DTOs.ProductImage;

namespace LearnCSharp.Application.Interfaces
{
    public interface IProductImageService
    {
        Task<IEnumerable<ProductImageDTO>> GetListProductImageByIdAsync(int id);
        Task DeleteAsync(int id);
    }
}