using HillMetrics.Core.Storage.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HillMetrics.MIND.Infrastructure.Database.Database
{
    /// <summary>
    /// Apply EF core migrations + table seed on normalized database
    /// </summary>
    public static class MigrationMindManager
    {
        public static IHost MigrateMindAppDatabase(this IHost host, ILogger logger)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<MindApplicationContext>();

                _ = MigrationsManager<MindApplicationContext>.MigrateDatabase(dbContext, logger);

                return host;
            }
        }
    }
}
