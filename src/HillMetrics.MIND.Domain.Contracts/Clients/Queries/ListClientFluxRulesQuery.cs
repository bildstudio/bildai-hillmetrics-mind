using FluentResults;
using HillMetrics.Core.Common;
using HillMetrics.Core.Mediator;
using MediatR;

namespace HillMetrics.MIND.Domain.Contracts.Clients.Queries
{
    public class ListClientFluxRulesQuery : AuditedCommand<Result<PagedResponse<ClientFluxRule>>>
    {
        public ListClientFluxRulesQuery(int clientId)
        {
            ClientId = clientId;
        }

        public int ClientId { get; set; }
    }
}
