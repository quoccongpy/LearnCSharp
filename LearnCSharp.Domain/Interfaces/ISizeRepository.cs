using LearnCSharp.Domain.Entities;

namespace LearnCSharp.Domain.Interfaces
{
    public interface ISizeRepository : IRepository<Size>
    {
        void Update(Size size);
    }
}