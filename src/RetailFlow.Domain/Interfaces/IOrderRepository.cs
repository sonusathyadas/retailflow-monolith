using System.Collections.Generic;
using RetailFlow.Domain.Entities;

namespace RetailFlow.Domain.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        IEnumerable<Order> GetOrdersByUserId(int userId);
        Order GetOrderWithItems(int orderId);
    }
}
