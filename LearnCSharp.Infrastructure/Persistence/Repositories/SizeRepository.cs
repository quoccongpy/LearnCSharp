using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;

namespace LearnCSharp.Infrastructure.Persistence.Repositories
{
    public class SizeRepository : Repository<Size>, ISizeRepository
    {
        private readonly LearnCSharpDbContext _context;

        public SizeRepository(LearnCSharpDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Size size)
        {
            _context.Update(size);
        }
    }
}