using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;

namespace LearnCSharp.Infrastructure.Persistence.Repositories
{
    public class OrderDetailRepository : Repository<OrderDetails>, IOrderDetailRepository
    {
        private readonly LearnCSharpDbContext _context;
        public OrderDetailRepository(LearnCSharpDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OrderDetails orderDetails)
        {
            _context.Update(orderDetails);
        }
    }
}