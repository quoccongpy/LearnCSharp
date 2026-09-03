using LearnCSharp.Domain.Entities;

namespace LearnCSharp.Domain.Interfaces
{
    public interface IProductReviewRepository : IRepository<ProductReview>
    {
        Task<bool> HasUserPurchasedAndDeliveredAsync(Guid userId, int productId);
        Task<bool> HasUserAlreadyReviewedAsync(Guid userId, int productId);
        Task<(List<ProductReview> Data, int TotalCount)> GetByProductIdPagedAsync(int productId, int skip, int take, bool includeHidden = false);
        void Update(ProductReview productReview);
    }
}