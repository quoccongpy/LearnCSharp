using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;

namespace LearnCSharp.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        private readonly LearnCSharpDbContext _context;

        public RefreshTokenRepository(LearnCSharpDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(RefreshToken refreshToken)
        {
            _context.Update(refreshToken);
        }
    }
}