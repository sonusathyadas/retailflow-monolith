using System.Collections.Generic;
using RetailFlow.Application.DTOs;

namespace RetailFlow.Application.Services
{
    public interface IOrderService
    {
        OrderDto CreateOrder(int userId, CreateOrderRequest request);
        OrderDto GetById(int orderId);
        IEnumerable<OrderDto> GetByUserId(int userId);
        OrderDto CancelOrder(int orderId, int requestingUserId);
    }
}
