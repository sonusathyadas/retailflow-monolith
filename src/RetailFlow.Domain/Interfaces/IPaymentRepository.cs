using RetailFlow.Domain.Entities;

namespace RetailFlow.Domain.Interfaces
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Payment GetByOrderId(int orderId);
    }
}
