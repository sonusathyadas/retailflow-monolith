using System;

namespace RetailFlow.Domain.Events
{
    public class OrderCreatedEvent
    {
        public string OrderId { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }
}
