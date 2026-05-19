using System;
using RetailFlow.Application.DTOs;
using RetailFlow.Domain.Entities;
using RetailFlow.Domain.Interfaces;
using Serilog;

namespace RetailFlow.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepo;
        private static readonly ILogger _log = Log.ForContext<InventoryService>();

        public InventoryService(IInventoryRepository inventoryRepo)
        {
            _inventoryRepo = inventoryRepo;
        }

        public InventoryDto GetByProductId(string productId)
        {
            var inv = _inventoryRepo.GetByProductId(productId);
            return inv == null ? null : MapToDto(inv);
        }

        public InventoryDto UpdateInventory(UpdateInventoryRequest request)
        {
            var inv = _inventoryRepo.GetByProductId(request.ProductId);

            if (inv == null)
            {
                inv = new Inventory
                {
                    ProductId = request.ProductId,
                    QuantityAvailable = request.QuantityAvailable,
                    WarehouseLocation = request.WarehouseLocation,
                    LowStockThreshold = request.LowStockThreshold,
                    UpdatedAt = DateTime.UtcNow
                };
                _inventoryRepo.Add(inv);
            }
            else
            {
                inv.QuantityAvailable = request.QuantityAvailable;
                inv.WarehouseLocation = request.WarehouseLocation;
                inv.LowStockThreshold = request.LowStockThreshold;
                inv.UpdatedAt = DateTime.UtcNow;
                _inventoryRepo.Update(inv);
            }

            _inventoryRepo.SaveChanges();

            if (inv.QuantityAvailable <= inv.LowStockThreshold)
                _log.Warning("Low stock alert for product {ProductId}: {Qty} remaining", request.ProductId, inv.QuantityAvailable);

            return MapToDto(inv);
        }

        public bool ReserveStock(ReserveStockRequest request)
        {
            var result = _inventoryRepo.ReserveStock(request.ProductId, request.Quantity);
            if (!result)
                _log.Warning("Stock reservation failed for product {ProductId}, qty {Qty}", request.ProductId, request.Quantity);
            return result;
        }

        private static InventoryDto MapToDto(Inventory inv)
        {
            return new InventoryDto
            {
                Id = inv.Id,
                ProductId = inv.ProductId,
                QuantityAvailable = inv.QuantityAvailable,
                WarehouseLocation = inv.WarehouseLocation,
                IsLowStock = inv.QuantityAvailable <= inv.LowStockThreshold
            };
        }
    }
}
