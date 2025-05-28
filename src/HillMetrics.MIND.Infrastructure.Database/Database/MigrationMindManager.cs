using HillMetrics.Core.Exceptions;
using HillMetrics.Core.Search;
using HillMetrics.Core.Storage.Database;
using HillMetrics.MIND.Domain.Contracts.Services;
using HillMetrics.MIND.Infrastructure.Contracts.Services;
using HillMetrics.Refined.Infrastructure.Database.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

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


        public static async Task<IHost> MigrateMindAppClientsDatabasesAsync(this IHost host, ILogger logger)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            var refinedBuilder = services.GetRequiredService<IRefinedDbBuilder>();
            var mindAppDbContext = services.GetRequiredService<MindApplicationContext>();

            //select only active clients
            var clientsIds = mindAppDbContext.Clients.Where(s => !s.IsDeleted && s.IsActive).Select(s => s.Id).ToList();
            foreach (var clientId in clientsIds)
            {
                try
                {
                    var contextResult = await refinedBuilder.CreateDbContextAsync<FinancialRefinedContext>(clientId, CancellationToken.None);
                    if (contextResult.IsFailed)
                    {
                        logger.LogError("Failed to create a dbContext for clientId: {clientId}", clientId);
                        throw new InternalServerException($"Failed to create a dbContext for clientId: {clientId}");
                    }

                    using var context = contextResult.Value;
                    MigrationsManager<FinancialRefinedContext>.MigrateDatabase(context, logger);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to migrate refined db for a clientId: {clientId}, error: {ExceptionMessage}", clientId, ex.Message);
                    throw;
                }
            }

            return host;
        }
    }
}
