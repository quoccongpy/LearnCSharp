using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;

namespace LearnCSharp.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly LearnCSharpDbContext _context;
        public CategoryRepository(LearnCSharpDbContext context) : base(context)
        {
            _context=context;
        }
        public  void Update(Category category)
        {
             _context.Update(category);
        }
    }
}