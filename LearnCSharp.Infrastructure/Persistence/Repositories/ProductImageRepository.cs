using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;

namespace LearnCSharp.Infrastructure.Persistence.Repositories
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private readonly LearnCSharpDbContext _context;

        public ProductImageRepository(LearnCSharpDbContext context) : base(context)
        {
            _context = context;
        }
    }
}