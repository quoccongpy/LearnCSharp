using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;

namespace LearnCSharp.Infrastructure.Persistence.Repositories
{
    public class CrustRepository : Repository<Crust>, ICrustRepository
    {
        private readonly LearnCSharpDbContext _context;

        public CrustRepository(LearnCSharpDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Crust crust)
        {
            _context.Update(crust);
        }
    }
}