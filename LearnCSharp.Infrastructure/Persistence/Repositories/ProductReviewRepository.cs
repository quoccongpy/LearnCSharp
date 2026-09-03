using LearnCSharp.Application.Utility;
using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearnCSharp.Infrastructure.Persistence.Repositories
{
    public class ProductReviewRepository : Repository<ProductReview>, IProductReviewRepository
    {
        private readonly LearnCSharpDbContext _context;
        public ProductReviewRepository(LearnCSharpDbContext context) : base(context)
        {
            _context= context;
        }

        public async Task<(List<ProductReview> Data, int TotalCount)> GetByProductIdPagedAsync(int productId, int skip, int take, bool includeHidden = false)
        {
            IQueryable<ProductReview> query = _context.ProductReview.Where(r => r.ProductId == productId);
            if (!includeHidden)
            {
                query = query.Where(r => !r.IsHidden);
            }
            query = query.OrderByDescending(r => r.CreatedAt);
            var totalCount = await query.CountAsync();
            var data = await query.Skip(skip).Take(take).ToListAsync();
            return (data, totalCount);
        }

        public async Task<bool> HasUserAlreadyReviewedAsync(Guid userId, int productId)
        {
            return await _context.ProductReview.AnyAsync(r => r.UserId == userId && r.ProductId == productId);
        }

        public async Task<bool> HasUserPurchasedAndDeliveredAsync(Guid userId, int productId)
        {
            var result= await _context.Order.Where(o => o.UserId == userId && o.Status == SD.Delivered).AnyAsync(o => o.OrderDetails.Any(d => d.ProductId == productId));
            return result;
        }

        public void Update(ProductReview productReview)
        {
            _context.Update(productReview);
        }
    }
}