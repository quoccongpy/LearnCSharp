using LearnCSharp.Domain.Entities;

namespace LearnCSharp.Domain.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        void Update(Order order);
    }
}