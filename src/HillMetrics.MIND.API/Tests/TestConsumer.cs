
//using Asp.Versioning;
using HillMetrics.Core.Contracts;
using HillMetrics.Core.Messaging.Bus;
using MassTransit;


namespace HillMetrics.MIND.API.Tests;

public class TestConsumer : BaseEventConsumer<TestConsumerEvent>
{
    private readonly IConsumerDataSingleton _consumerDataSingleton;
    public TestConsumer(
        ILogger<TestConsumer> logger,
        ICorrelationService correlationService,
        IConsumerDataSingleton consumerDataSingleton
        ) : base(logger, correlationService)
    {
        _consumerDataSingleton = consumerDataSingleton;
    }
    protected async override Task ConsumeInner(ConsumeContext<TestConsumerEvent> context)
    {
        _consumerDataSingleton.Message = context.Message?.Message;
    }
}

public record TestConsumerEvent
{
    public TestConsumerEvent()
    {

    }
    public TestConsumerEvent(string message)
    {
        Message = message;
    }

    public string Message { get; set; }
}
