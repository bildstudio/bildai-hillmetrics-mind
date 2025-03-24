using HillMetrics.Core.Messaging.Notification.Realtime;
using MassTransit;

namespace HillMetrics.MIND.API.Consumers
{
    public class SignalrSubscribeEventConsumerTest : IConsumer<SignalrSubscribeEvent>
    {
        private readonly ILogger<SignalrSubscribeEventConsumerTest> _logger;

        public SignalrSubscribeEventConsumerTest(ILogger<SignalrSubscribeEventConsumerTest> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SignalrSubscribeEvent> context)
        {
            _logger.LogInformation("Consumed message, {msg}", context.Message);

            await Task.CompletedTask;
        }
    }
}
