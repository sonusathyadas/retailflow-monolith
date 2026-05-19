using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Serilog;

namespace RetailFlow.Messaging.Publishers
{
    /// <summary>
    /// RabbitMQ implementation of IEventPublisher.
    /// Publishes events to a topic exchange using the event type name as routing key.
    /// </summary>
    public class RabbitMqEventPublisher : IEventPublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private const string ExchangeName = "retailflow.events";
        private static readonly ILogger _log = Log.ForContext<RabbitMqEventPublisher>();

        public RabbitMqEventPublisher(string connectionString)
        {
            var factory = new ConnectionFactory { Uri = new Uri(connectionString) };
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

            // Declare a durable topic exchange
            _channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false
            ).GetAwaiter().GetResult();
        }

        public void Publish<T>(T @event) where T : class
        {
            var routingKey = typeof(T).Name;
            var body = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event)));

            var props = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                Type = routingKey,
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            _channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: props,
                body: body
            ).GetAwaiter().GetResult();

            _log.Information("Published event {EventType} to exchange {Exchange}", routingKey, ExchangeName);
        }

        public void Dispose()
        {
            _channel?.CloseAsync().GetAwaiter().GetResult();
            _connection?.CloseAsync().GetAwaiter().GetResult();
        }
    }
}
