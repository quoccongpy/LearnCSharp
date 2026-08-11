using LearnCSharp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace LearnCSharp.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LearnCSharpDbContext _context;
        private IDbContextTransaction _transaction;
        private bool _disposed;

        public UnitOfWork(LearnCSharpDbContext context)
        {
            _context = context;
            Category = new CategoryRepository(context);
            Product = new ProductRepository(context);
            ProductImage = new ProductImageRepository(context);
            RefreshToken = new RefreshTokenRepository(context);
            Order = new OrderRepository(context);
            OrderDetail = new OrderDetailRepository(context);
            Size = new SizeRepository(context);
            Crust = new CrustRepository(context);
            ProductVariant = new ProductVariantRepository(context);
            Payment = new PaymentRepository(context);
            Notification = new NotificationRepository(context);
        }

        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public IProductImageRepository ProductImage { get; private set; }
        public IRefreshTokenRepository RefreshToken { get; private set; }
        public IOrderRepository  Order { get; private set; }
        public IOrderDetailRepository  OrderDetail { get; private set; }
        public ISizeRepository  Size { get; private set; }
        public ICrustRepository Crust { get; private set; }
        public IProductVariantRepository ProductVariant { get; private set; }
        public IPaymentRepository Payment { get; private set; }
        public INotificationRepository Notification { get; private set; }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _transaction?.CommitAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                await _transaction?.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.Dispose();
                _transaction?.Dispose();
            }
            _disposed = true;
        }
    }
}