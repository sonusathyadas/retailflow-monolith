using System.ComponentModel.DataAnnotations;

namespace RetailFlow.Application.DTOs
{
    public class ReserveStockRequest
    {
        [Required]
        public string ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    public class InventoryDto
    {
        public int Id { get; set; }
        public string ProductId { get; set; }
        public int QuantityAvailable { get; set; }
        public string WarehouseLocation { get; set; }
        public bool IsLowStock { get; set; }
    }

    public class UpdateInventoryRequest
    {
        [Required]
        public string ProductId { get; set; }

        [Required]
        public string WarehouseLocation { get; set; }

        [Range(0, int.MaxValue)]
        public int QuantityAvailable { get; set; }

        public int LowStockThreshold { get; set; } = 10;
    }
}
