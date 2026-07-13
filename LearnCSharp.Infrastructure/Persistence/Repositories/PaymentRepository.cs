using LearnCSharp.Domain.Entities;
using LearnCSharp.Domain.Interfaces;

namespace LearnCSharp.Infrastructure.Persistence.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        private readonly LearnCSharpDbContext _context;
        public PaymentRepository(LearnCSharpDbContext context) : base(context)
        {
            _context= context;
        }

        public void Update(Payment payment)
        {
            _context.Update(payment);
        }
    }
}