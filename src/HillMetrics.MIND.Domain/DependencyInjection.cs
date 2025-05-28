using HillMetrics.Core.Mediator.Extensions;
using HillMetrics.MIND.Domain.UseCase.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using HillMetrics.Business.API.SDK;
using HillMetrics.Core.Authentication.Keycloak.HttpHandlers;

namespace HillMetrics.MIND.Domain;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddHillMetricsMINDServices(
        this IHostApplicationBuilder builder)
    {

        builder.Services.AddMediatRAndPipelineBehaviors([typeof(CreateClientCommandHandler).Assembly]);
        string businessApiEndpoint = builder.Configuration.GetValue<string>("Services:BusinessApi", "https+http://BusinessAPI") ?? "https+http://BusinessAPI";
        builder.Services.AddBusinessApiSDK<PrivateAuthenticationHttpHandler>(businessApiEndpoint, "MindAPI", httpClientTimeout: TimeSpan.FromMinutes(2));


        return builder;
    }
}
