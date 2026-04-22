using System.Linq.Expressions;

namespace LearnCSharp.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, params Expression<Func<T, object>>[] includes);

        Task<(List<T> Data, int TotalCount)> GetPagedAsync(Expression<Func<T, bool>> filter, int skip, int take, bool tracked = false, params Expression<Func<T, object>>[] includes);

        Task<T> GetByIdIncludeAsync(Expression<Func<T, bool>> filter, bool tracked = true, params Expression<Func<T, object>>[] includes);

        Task<T> GetByIdAsync(int id);

        Task<T> GetByfilterAsync(Expression<Func<T, bool>> filter, bool tracked = true);

        Task CreateAsync(T entity);

        Task RemoveAsync(T entity);
        Task<List<TResult>> GetAsync<TResult>(Expression<Func<T, bool>> filter,Expression<Func<T, TResult>> selector, int? take = null, bool tracked = true);
    }
}