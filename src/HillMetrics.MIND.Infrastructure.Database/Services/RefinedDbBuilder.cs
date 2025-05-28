using FluentResults;
using HillMetrics.Core.Contracts;
using HillMetrics.Core.Errors;
using HillMetrics.Core.Storage.Database;
using HillMetrics.Core.Storage.Database.Contracts;
using HillMetrics.MIND.Domain.Contracts.Clients;
using HillMetrics.MIND.Domain.Contracts.Services;
using HillMetrics.MIND.Infrastructure.Contracts.Services;
using HillMetrics.Refined.Infrastructure.Database.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HillMetrics.MIND.Infrastructure.Database.Services
{
    public class RefinedDbBuilder : IRefinedDbBuilder
    {
        private readonly ILogger<RefinedDbBuilder> _logger;
        private readonly IClientService _clientService;
        private readonly IHillMetricsDbContextFactory _dbContextFactory;
        private readonly ITimeProvider _timeProvider;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConnectionStringsConfig _connectionStringsConfig;
        public RefinedDbBuilder(
            ILogger<RefinedDbBuilder> logger,
            IClientService clientService,
            IHillMetricsDbContextFactory dbContextFactory,
            ITimeProvider timeProvider,
            ILoggerFactory loggerFactory,
            IConnectionStringsConfig connectionStringsConfig)
        {
            _logger = logger;
            _clientService = clientService;
            _dbContextFactory = dbContextFactory;
            _timeProvider = timeProvider;
            _loggerFactory = loggerFactory;
            _connectionStringsConfig = connectionStringsConfig;
        }

        public async Task<Result<TContext>> CreateDbContextAsync<TContext>(int clientId, CancellationToken cancellationToken) where TContext : DbContext
        {
            try
            {
                if (typeof(TContext) != typeof(FinancialRefinedContext))
                    return Result.Fail(new InternalServerError("TContext must be of type: 'FinancialRefinedContext'"));

                //return default RefinedDbContext
                if (clientId <= 0)
                {
                    string refinedConnString = _connectionStringsConfig.GetRefinedConnString();
                    return _dbContextFactory.Create<TContext>(refinedConnString, _timeProvider, _loggerFactory.CreateLogger<FinancialRefinedContext>());
                }

                var clientResult = await _clientService.GetAsync(clientId, cancellationToken);
                if (clientResult.IsFailed)
                    return clientResult.ToResult();

                ClientEntity clientInfo = clientResult.Value;

                string? connectionString = _connectionStringsConfig.GetProvisioningConnString();
                if (string.IsNullOrEmpty(connectionString))
                    return Result.Fail("'ConnectionStrings:Provision' is empty");

                string connString = PostgreConnStringBuilder.BuildConnectionString(connectionString, clientInfo.RefinedDbName);

                var context = _dbContextFactory.Create<TContext>(connString, _timeProvider, _loggerFactory.CreateLogger<FinancialRefinedContext>());

                return context;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateDbContextAsync error: {ExceptionMessage}", ex.Message);
                return Result.Fail(new InternalServerError($"Error creating a db context for client: {clientId}, error: {ex.Message}"));
            }
        }
    }
}
