using HillMetrics.Core.Mediator.Extensions;
using HillMetrics.MIND.Domain.UseCase.Clients;
using Microsoft.Extensions.Hosting;

namespace HillMetrics.MIND.Domain;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddHillMetricsMINDServices(
        this IHostApplicationBuilder builder)
    {

        builder.Services.AddMediatRAndPipelineBehaviors([typeof(CreateClientCommandHandler).Assembly]);
        


        return builder;
    }
}
