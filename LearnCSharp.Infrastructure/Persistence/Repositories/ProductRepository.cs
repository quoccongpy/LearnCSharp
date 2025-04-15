using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;

namespace LearnCSharp.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly LearnCSharpDbContext _context;

        public ProductRepository(LearnCSharpDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
            product.UpdatedDate = DateTime.Now; 
            _context.Update(product);
        }
    }
}