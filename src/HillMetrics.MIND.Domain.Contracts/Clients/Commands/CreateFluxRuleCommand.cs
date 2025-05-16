using FluentResults;
using HillMetrics.Core.Mediator;
using HillMetrics.MIND.Domain.Contracts.Clients.Models;
using MediatR;

namespace HillMetrics.MIND.Domain.Contracts.Clients.Commands
{
    public class CreateFluxRuleCommand : AuditedCommand<Result<ClientFluxRule>>
    {
        public SaveClientFluxRuleModel Model { get; }
        public CreateFluxRuleCommand(SaveClientFluxRuleModel model)
        {
            Model = model;
        }
    }
}
