using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearnCSharp.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly LearnCSharpDbContext _context;

        public ProductRepository(LearnCSharpDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<(List<Product> Data, int TotalCount)> SearchAsync(string keyword, int? categoryId, int skip, int take)
        {
            var query = _context.Product.Include(p => p.Category).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var searchKeyword = keyword.Trim();
                query = query.Where(p => EF.Functions.ILike(EF.Functions.Collate(p.Name, "und-x-icu"), $"%{searchKeyword}%"));
            }
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }
            var totalCount = await query.CountAsync();
            var data = await query.Skip(skip).Take(take).ToListAsync();
            return (data, totalCount);
        }

        public void Update(Product product)
        {
            product.UpdatedDate = DateTime.Now;
            _context.Update(product);
        }
    }
}