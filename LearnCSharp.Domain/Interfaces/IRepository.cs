using LearnCSharp.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq.Expressions;

namespace LearnCSharp.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T,bool>>? filter=null,bool tracked=true);
        Task<(List<T> Data, int TotalCount)> GetPagedAsync(Expression<Func<T, bool>> filter,int skip,int take, bool tracked = false, params Expression<Func<T, object>>[] includes);
        //Task<T> GetByIdAsync(Expression<Func<T, bool>> filter, bool tracked = true);
        Task<T> GetByIdAsync(int id);
        Task<T> GetByfilterAsync(Expression<Func<T, bool>> filter, bool tracked = true);
        Task CreateAsync(T entity);
        Task RemoveAsync(T entity);
    }
}