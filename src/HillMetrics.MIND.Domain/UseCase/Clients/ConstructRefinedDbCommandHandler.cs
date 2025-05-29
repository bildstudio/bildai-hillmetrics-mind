using FluentResults;
using HillMetrics.Core.Errors;
using HillMetrics.Core.Mediator;
using HillMetrics.MIND.Domain.Contracts.Clients.Commands;
using HillMetrics.MIND.Infrastructure.Contracts.Services;
using HillMetrics.Refined.Infrastructure.Database.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HillMetrics.MIND.Domain.UseCase.Clients
{
    public class ConstructRefinedDbCommandHandler : Handler<ConstructRefinedDbCommandHandler, bool, ConstructRefinedDbCommand>
    {
        private readonly IRefinedDbBuilder _refinedDbBuilder;
        public ConstructRefinedDbCommandHandler(
            ILogger<ConstructRefinedDbCommandHandler> logger,
            IRefinedDbBuilder refinedDbBuilder)
            : base(logger)
        {
            _refinedDbBuilder = refinedDbBuilder;
        }

        public override async Task<Result<bool>> HandleInnerAsync(ConstructRefinedDbCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request.Audit?.SetEventData(Core.AuditEventType.Created, request.ClientId.ToString(), "ConstructRefinedDb", $"Constructing refined db for client: {request.ClientId}");

                var contextResult = await _refinedDbBuilder.CreateDbContextAsync<FinancialRefinedContext>(request.ClientId, cancellationToken);
                if (contextResult.IsFailed)
                    return contextResult.ToResult<bool>();

                using var context = contextResult.Value;
                await context.Database.MigrateAsync(cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HandleInnerAsync error: {ExceptionMessage}", ex.Message);
                return Result.Fail(new InternalServerError(ex.Message));
            }
        }
    }
}
