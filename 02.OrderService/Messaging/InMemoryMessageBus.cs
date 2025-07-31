using System;
using _01.Contracts.Messaging;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace _02.OrderService.Messaging
{
    public class InMemoryMessageBus : IMessageBus
    {
        private readonly ILogger<InMemoryMessageBus> _logger;

        public InMemoryMessageBus(ILogger<InMemoryMessageBus> logger)
        {
            _logger = logger;
        }
        public Task PublishAsync(string topic, object payload)
        {
            // Simple stub: log the event. Later you can replace with RabbitMQ/Azure Service Bus.
            var serialized = JsonSerializer.Serialize(payload);
            _logger.LogInformation("Published message to topic '{Topic}': {Payload}", topic, serialized);
            return Task.CompletedTask;
        }
    }
}
