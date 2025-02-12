//using Asp.Versioning;

using HillMetrics.Core.Messaging.Bus.Extensions;

namespace HillMetrics.MIND.API.Tests;

public static class TestExtensions
{
    public static IServiceCollection AddTestServices(this IServiceCollection services)
    {

        services.AddSingleton<IConsumerDataSingleton, ConsumerDataSingleton>();

        //services.AddHillMetricsRabbitMq(cfg =>
        //{
        //    cfg.AddConsumer<TestConsumer>();
        //});

        return services;
    }
}