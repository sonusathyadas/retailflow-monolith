using System;

namespace RetailFlow.Domain.Events
{
    public class PaymentFailedEvent
    {
        public string PaymentId { get; set; }
        public string OrderId { get; set; }
        public string Reason { get; set; }
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }
}
