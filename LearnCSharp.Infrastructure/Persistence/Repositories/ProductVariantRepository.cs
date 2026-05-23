using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LearnCSharp.Infrastructure.Persistence.Repositories
{
    public class ProductVariantRepository : Repository<ProductVariant>, IProductVariantRepository
    {
        private readonly LearnCSharpDbContext _context;

        public ProductVariantRepository(LearnCSharpDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetProductsHasVariantsAsync()
        {
            var products = await _context.Product.Where(p => p.ProductVariants.Any())
                                                 .AsNoTracking()
                                                 .ToListAsync();
            return products;
        }

        public void Update(ProductVariant productVariant)
        {
            _context.Update(productVariant);
        }
    }
}