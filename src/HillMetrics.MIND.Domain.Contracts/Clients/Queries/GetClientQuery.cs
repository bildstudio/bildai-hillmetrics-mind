using FluentResults;
using HillMetrics.Core.Mediator;
using MediatR;

namespace HillMetrics.MIND.Domain.Contracts.Clients.Queries
{
    public class GetClientQuery : AuditedCommand<Result<ClientEntity>>
    {
        public int Id { get; }

        public GetClientQuery(int id)
        {
            Id = id;
        }
    }
}
