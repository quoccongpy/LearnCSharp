using LearnCSharp.Domain.Entities;
using System.Linq.Expressions;

namespace LearnCSharp.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T,bool>>? filter=null,bool tracked=true);
        Task<(List<T> Data, int TotalCount)> GetPagedAsync(Expression<Func<T, bool>> filter,int skip,int take, bool tracked = false);
        Task<T> GetByIdAsync(Expression<Func<T, bool>> filter, bool tracked = true);
        Task CreateAsync(T entity);
        Task RemoveAsync(T entity);
    }
}