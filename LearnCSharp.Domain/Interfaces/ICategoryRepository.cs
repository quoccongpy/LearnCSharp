using LearnCSharp.Domain.Entities;

namespace LearnCSharp.Domain.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category category); 
    }
}