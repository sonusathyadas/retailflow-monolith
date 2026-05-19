using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using RetailFlow.Domain.Entities;
using RetailFlow.Domain.Interfaces;
using RetailFlow.Infrastructure.Data;

namespace RetailFlow.Infrastructure.Repositories
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(RetailFlowDbContext context) : base(context) { }

        public IEnumerable<Order> GetOrdersByUserId(int userId)
        {
            return _dbSet
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();
        }

        public Order GetOrderWithItems(int orderId)
        {
            return _dbSet
                .Include(o => o.OrderItems)
                .Include(o => o.User)
                .FirstOrDefault(o => o.Id == orderId);
        }
    }
}
