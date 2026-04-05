using LearnCSharp.Domain.Entities;

namespace LearnCSharp.Domain.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);
        Task<(List<Product> Data, int TotalCount)> SearchAsync(string? keyword,int? categoryId, int skip,int take);
    }
}