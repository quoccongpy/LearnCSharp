using LearnCSharp.Domain.Entities;

namespace LearnCSharp.Domain.Interfaces
{
    public interface ICrustRepository : IRepository<Crust>
    {
        void Update(Crust crust);
    }
}