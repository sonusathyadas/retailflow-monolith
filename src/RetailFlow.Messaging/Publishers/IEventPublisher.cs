namespace RetailFlow.Messaging.Publishers
{
    /// <summary>
    /// Abstraction over the messaging transport (RabbitMQ / Azure Service Bus).
    /// Decouples application logic from the specific broker implementation.
    /// </summary>
    public interface IEventPublisher
    {
        void Publish<T>(T @event) where T : class;
    }
}
