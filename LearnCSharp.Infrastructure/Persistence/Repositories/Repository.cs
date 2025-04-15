using LearnCSharp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace LearnCSharp.Infrastructure.Persistence.Repositories
{

    //https://medium.com/@codebob75/repository-pattern-c-ultimate-guide-entity-framework-core-clean-architecture-dtos-dependency-6a8d8b444dcb
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly LearnCSharpDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(LearnCSharpDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, bool tracked = true)
        {
            IQueryable<T> query = _dbSet;
            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(Expression<Func<T, bool>> filter, bool tracked = true)
        {
            IQueryable<T> query = _dbSet;

            if (!tracked)
            {
                query = query.AsNoTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<(List<T> Data, int TotalCount)> GetPagedAsync(Expression<Func<T, bool>> filter, int skip, int take, bool tracked = false)
        {
           var query = _dbSet.AsQueryable();
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            var totalCount = await query.CountAsync();
            var data = await query.Skip(skip).Take(take).ToListAsync();
            return(data, totalCount);
        }

        public async Task RemoveAsync(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}