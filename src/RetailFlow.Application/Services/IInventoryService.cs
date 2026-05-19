using RetailFlow.Application.DTOs;

namespace RetailFlow.Application.Services
{
    public interface IInventoryService
    {
        InventoryDto GetByProductId(string productId);
        InventoryDto UpdateInventory(UpdateInventoryRequest request);
        bool ReserveStock(ReserveStockRequest request);
    }
}
