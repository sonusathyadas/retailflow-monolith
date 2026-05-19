using RetailFlow.Domain.Entities;

namespace RetailFlow.Domain.Interfaces
{
    public interface IInventoryRepository : IRepository<Inventory>
    {
        Inventory GetByProductId(string productId);
        bool ReserveStock(string productId, int quantity);
        bool DeductStock(string productId, int quantity);
    }
}
