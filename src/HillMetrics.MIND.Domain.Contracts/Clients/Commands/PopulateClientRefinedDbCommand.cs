using FluentResults;
using HillMetrics.Core.Mediator;

namespace HillMetrics.MIND.Domain.Contracts.Clients.Commands
{
    //PopulateClientRefinedDb
    public class PopulateClientRefinedDbCommand : AuditedCommand<Result<bool>>
    {
        public int ClientId { get; set; }
        public PopulateClientRefinedDbCommand(int clientId)
        {
            ClientId = clientId;
        }
    }
}
