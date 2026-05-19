using RetailFlow.Domain.Events;
using Serilog;

namespace RetailFlow.Messaging.Consumers
{
    /// <summary>
    /// Listens for PaymentCompleted events and triggers inventory reservation.
    /// </summary>
    public class InventoryConsumer : BaseConsumer<PaymentCompletedEvent>
    {
        private static readonly ILogger _log = Log.ForContext<InventoryConsumer>();

        public InventoryConsumer(string connectionString)
            : base(connectionString, "retailflow.inventory.queue") { }

        protected override void Handle(PaymentCompletedEvent message)
        {
            _log.Information("InventoryConsumer received PaymentCompleted for order {OrderId}",
                message.OrderId);
            // Inventory reservation logic is triggered here
        }
    }
}
