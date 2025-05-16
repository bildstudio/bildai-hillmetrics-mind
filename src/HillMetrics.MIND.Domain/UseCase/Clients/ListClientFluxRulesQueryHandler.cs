using FluentResults;
using HillMetrics.Core.Authentication;
using HillMetrics.Core.Common;
using HillMetrics.Core.Errors;
using HillMetrics.Core.Mediator;
using HillMetrics.MIND.Domain.Contracts.Clients;
using HillMetrics.MIND.Domain.Contracts.Clients.Queries;
using HillMetrics.MIND.Domain.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace HillMetrics.MIND.Domain.UseCase.Clients
{
    public class ListClientFluxRulesQueryHandler : Handler<ListClientFluxRulesQueryHandler, PagedResponse<ClientFluxRule>, ListClientFluxRulesQuery>
    {
        private readonly IClientService _clientService;
        public ListClientFluxRulesQueryHandler(
            ILogger<ListClientFluxRulesQueryHandler> logger,
            IClientService clientService)
            : base(logger)
        {
            _clientService = clientService;
        }

        public override async Task<Result<PagedResponse<ClientFluxRule>>> HandleInnerAsync(ListClientFluxRulesQuery request, CancellationToken cancellationToken)
        {
            if (!(request.Audit.User.IsInRole(Roles.Mind.ManageClients) || await _clientService.UserHasClientAccessAsync(request.Audit.User.Id!, request.ClientId, cancellationToken)))
                return Result.Fail(new ForbidenError("You do not have access to this client."));

            return await _clientService.ListClientFluxRulesAsync(request.ClientId, cancellationToken);
        }
    }
}
