using System;
using System.Collections.Generic;

namespace RetailFlow.Domain.Events
{
    public class InventoryReservedEvent
    {
        public string OrderId { get; set; }
        public List<ReservedItem> ReservedItems { get; set; } = new List<ReservedItem>();
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }

    public class ReservedItem
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
