using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RetailFlow.Domain.Enums;

namespace RetailFlow.Domain.Entities
{
    [Table("Payments")]
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        public PaymentStatus Status { get; set; } = PaymentStatus.Initiated;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [MaxLength(200)]
        public string TransactionReference { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public int RetryCount { get; set; } = 0;
    }
}
