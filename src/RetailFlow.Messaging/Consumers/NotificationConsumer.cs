using RetailFlow.Domain.Events;
using Serilog;

namespace RetailFlow.Messaging.Consumers
{
    /// <summary>
    /// Listens for PaymentCompleted and InventoryReserved events to send notifications.
    /// </summary>
    public class NotificationConsumer : BaseConsumer<InventoryReservedEvent>
    {
        private static readonly ILogger _log = Log.ForContext<NotificationConsumer>();

        public NotificationConsumer(string connectionString)
            : base(connectionString, "retailflow.notification.queue") { }

        protected override void Handle(InventoryReservedEvent message)
        {
            _log.Information("NotificationConsumer received InventoryReserved for order {OrderId}",
                message.OrderId);
            // Notification dispatch logic triggered here
        }
    }
}
