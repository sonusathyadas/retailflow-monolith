using System.Linq;
using RetailFlow.Domain.Entities;
using RetailFlow.Domain.Interfaces;
using RetailFlow.Infrastructure.Data;

namespace RetailFlow.Infrastructure.Repositories
{
    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(RetailFlowDbContext context) : base(context) { }

        public Payment GetByOrderId(int orderId)
        {
            return _dbSet.FirstOrDefault(p => p.OrderId == orderId);
        }
    }
}
