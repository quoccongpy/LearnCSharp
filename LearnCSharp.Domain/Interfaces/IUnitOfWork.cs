namespace LearnCSharp.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        IProductImageRepository ProductImage { get; }
        IRefreshTokenRepository  RefreshToken { get; }
        IOrderRepository Order { get; }
        IOrderDetailRepository OrderDetail { get; }
        ISizeRepository Size { get; }
        ICrustRepository Crust { get; }
        IProductVariantRepository ProductVariant { get; }
        IPaymentRepository Payment { get; }
        INotificationRepository Notification { get; }
        IProductReviewRepository ProductReview { get; }

        Task<int> CompleteAsync();

        Task BeginTransactionAsync();

        Task CommitTransactionAsync();

        Task RollbackTransactionAsync();
    }
}