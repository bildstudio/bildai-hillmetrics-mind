using FluentResults;
using HillMetrics.Core.Common;
using HillMetrics.MIND.Domain.Contracts.Clients.Models;
using MediatR;

namespace HillMetrics.MIND.Domain.Contracts.Clients.Queries
{
    public class SearchClientsQuery : IRequest<Result<PagedResponse<ClientEntity>>>
    {
        public SearchClientsModel Model { get; }

        public SearchClientsQuery(SearchClientsModel model)
        {
            Model = model;
        }
    }
}
