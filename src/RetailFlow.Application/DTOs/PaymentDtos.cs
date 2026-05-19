using System;
using System.ComponentModel.DataAnnotations;

namespace RetailFlow.Application.DTOs
{
    public class ProcessPaymentRequest
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public string PaymentMethod { get; set; }  // CARD, WALLET, etc.

        public string CardToken { get; set; }  // Mock token
    }

    public class PaymentDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string TransactionReference { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
