using FluentResults;
using HillMetrics.Core.Common;
using HillMetrics.Core.Mediator;
using HillMetrics.MIND.Domain.Contracts.Clients;
using HillMetrics.MIND.Domain.Contracts.Clients.Queries;
using HillMetrics.MIND.Domain.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace HillMetrics.Normalized.Domain.UseCase.Clients
{
    public class SearchClientsQueryHandler : Handler<SearchClientsQueryHandler, PagedResponse<ClientEntity>, SearchClientsQuery>
    {
        private readonly IClientService _clientService;
        public SearchClientsQueryHandler(
            ILogger<SearchClientsQueryHandler> logger,
            IClientService clientService) : base(logger)
        {
            _clientService = clientService;
        }

        public override Task<Result<PagedResponse<ClientEntity>>> HandleInnerAsync(SearchClientsQuery request, CancellationToken cancellationToken)
        {
            return _clientService.SearchAsync(request.Model, cancellationToken);
        }
    }
}
