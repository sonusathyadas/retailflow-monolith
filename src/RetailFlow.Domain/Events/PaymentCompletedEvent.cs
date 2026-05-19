using System;

namespace RetailFlow.Domain.Events
{
    public class PaymentCompletedEvent
    {
        public string PaymentId { get; set; }
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }
}
