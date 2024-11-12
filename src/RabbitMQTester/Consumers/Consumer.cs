using MassTransit;
using RabbitMQTester.RabbitMessages;

namespace RabbitMQTester
{
    internal class SampleMessageConsumer : IConsumer<SampleMessage>
    {
        public Task Consume(ConsumeContext<SampleMessage> context)
        {
            Message.WriteResult($"Received ({nameof(SampleMessageConsumer)}): {context.Message.Body}");
            return Task.CompletedTask;
        }
    }
}
