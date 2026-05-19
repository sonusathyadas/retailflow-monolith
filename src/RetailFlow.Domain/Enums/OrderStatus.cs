namespace RetailFlow.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 0,
        PaymentProcessing = 1,
        Paid = 2,
        InventoryReserved = 3,
        Shipped = 4,
        Delivered = 5,
        Cancelled = 6
    }
}
