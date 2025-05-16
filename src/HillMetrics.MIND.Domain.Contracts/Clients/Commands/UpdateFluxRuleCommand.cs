using FluentResults;
using HillMetrics.Core.Mediator;
using HillMetrics.MIND.Domain.Contracts.Clients.Models;
using MediatR;

namespace HillMetrics.MIND.Domain.Contracts.Clients.Commands
{
    public class UpdateFluxRuleCommand : AuditedCommand<Result<ClientFluxRule>>
    {
        public int FluxRuleId { get; set; }
        public SaveClientFluxRuleModel Model { get; }
        public UpdateFluxRuleCommand(int fluxRuleId, SaveClientFluxRuleModel model)
        {
            FluxRuleId = fluxRuleId;
            Model = model;
        }
    }
}
