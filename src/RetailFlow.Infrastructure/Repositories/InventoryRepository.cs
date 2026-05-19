using System;
using System.Linq;
using RetailFlow.Domain.Entities;
using RetailFlow.Domain.Interfaces;
using RetailFlow.Infrastructure.Data;
using Serilog;

namespace RetailFlow.Infrastructure.Repositories
{
    public class InventoryRepository : BaseRepository<Inventory>, IInventoryRepository
    {
        private static readonly ILogger _log = Log.ForContext<InventoryRepository>();

        public InventoryRepository(RetailFlowDbContext context) : base(context) { }

        public Inventory GetByProductId(string productId)
        {
            return _dbSet.FirstOrDefault(i => i.ProductId == productId);
        }

        public bool ReserveStock(string productId, int quantity)
        {
            var inventory = GetByProductId(productId);
            if (inventory == null || inventory.QuantityAvailable < quantity)
            {
                _log.Warning("Reserve stock failed for product {ProductId}. Requested: {Qty}, Available: {Avail}",
                    productId, quantity, inventory?.QuantityAvailable ?? 0);
                return false;
            }

            inventory.QuantityAvailable -= quantity;
            inventory.UpdatedAt = DateTime.UtcNow;
            Update(inventory);
            SaveChanges();

            _log.Information("Reserved {Qty} units for product {ProductId}", quantity, productId);
            return true;
        }

        public bool DeductStock(string productId, int quantity)
        {
            var inventory = GetByProductId(productId);
            if (inventory == null || inventory.QuantityAvailable < quantity)
                return false;

            inventory.QuantityAvailable -= quantity;
            inventory.UpdatedAt = DateTime.UtcNow;
            Update(inventory);
            SaveChanges();
            return true;
        }
    }
}
