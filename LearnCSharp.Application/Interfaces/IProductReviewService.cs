using LearnCSharp.Application.Models.DTOs.Review;
using LearnCSharp.Application.Models;

namespace LearnCSharp.Application.Interfaces
{
    public interface IProductReviewService
    {
        Task CreateAsync(ProductReviewCreateDTO model);
        Task UpdateAsync(int reviewId, ProductReviewUpdateDTO model);
        Task DeleteAsync(int reviewId);
        Task HideAsync(int reviewId, bool isHidden);
        Task<PagedResult<ProductReviewDTO>> GetByProductIdAsync(int productId, int pageIndex, int pageSize);
        Task<PagedResult<ProductReviewDTO>> GetAllForAdminAsync(int pageIndex, int pageSize, string? keyword);
        Task<bool> CanUserReviewAsync(int productId);
    }
}