using RetailFlow.Domain.Events;
using Serilog;

namespace RetailFlow.Messaging.Consumers
{
    /// <summary>
    /// Listens for OrderCreated events and triggers payment initiation.
    /// In the monolith this is wired internally; in microservices it becomes a standalone service.
    /// </summary>
    public class PaymentConsumer : BaseConsumer<OrderCreatedEvent>
    {
        private static readonly ILogger _log = Log.ForContext<PaymentConsumer>();

        public PaymentConsumer(string connectionString)
            : base(connectionString, "retailflow.payment.queue") { }

        protected override void Handle(OrderCreatedEvent message)
        {
            _log.Information("PaymentConsumer received OrderCreated for order {OrderId}, amount {Amount}",
                message.OrderId, message.TotalAmount);

            // In the monolith, the PaymentService is called directly via DI.
            // This consumer acts as the async trigger point for future extraction.
        }
    }
}
