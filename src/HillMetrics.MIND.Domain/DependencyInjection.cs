using HillMetrics.Core.Mediator.Extensions;
using HillMetrics.Normalized.Infrastructure.Database.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace HillMetrics.MIND.Domain;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddHillMetricsMINDServices(
        this IHostApplicationBuilder builder,
        string connectionStringKey = "FinancialNormalizedDb")
    {
        builder.AddNormalizedDatabaseProvider(
            connectionString: builder.Configuration.GetConnectionString(connectionStringKey),
            lifetime: Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped);

        builder.Services.AddMediatRAndPipelineBehaviors(typeof(DependencyInjection).Assembly);

        return builder;
    }
}
