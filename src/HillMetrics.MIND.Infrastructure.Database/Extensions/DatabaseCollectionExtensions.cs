using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HillMetrics.Core.Storage.Database;
using HillMetrics.Core.Storage.Extensions;
using HillMetrics.MIND.Infrastructure.Database.Database;
using HillMetrics.MIND.Infrastructure.Database.Services;
using HillMetrics.MIND.Domain.Contracts.Services;

namespace HillMetrics.MIND.Infrastructure.Database.Extensions
{
    /// <summary>
    /// Connect to MindApplication database
    /// </summary>
    public static class DatabaseCollectionExtensions
    {
        public static TBuilder AddMindAppDatabaseProvider<TBuilder>(
            this TBuilder builder,
            string? connectionString,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TBuilder : IHostApplicationBuilder
        {
            builder.AddDatabaseProvider<MindApplicationContext>(
                Orchestrator.ServicesNames.Services.MindApplicationDb,
                connectionString,
                lifetime);

            builder.Services.AddHillMetricsUnitOfWorkServices();

            return builder;
        }

        public static IServiceCollection AddClientsManagementServices(this IServiceCollection services)
        {
            services.AddScoped<IClientService, ClientService>();

            return services;
        }
    }
}
