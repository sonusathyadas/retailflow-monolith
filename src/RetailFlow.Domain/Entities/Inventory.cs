using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetailFlow.Domain.Entities
{
    [Table("Inventory")]
    public class Inventory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ProductId { get; set; }  // MongoDB ObjectId as string

        public int QuantityAvailable { get; set; }

        [MaxLength(200)]
        public string WarehouseLocation { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int LowStockThreshold { get; set; } = 10;
    }
}
