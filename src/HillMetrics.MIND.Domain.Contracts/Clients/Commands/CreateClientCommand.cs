using FluentResults;
using HillMetrics.Core.Mediator;
using HillMetrics.MIND.Domain.Contracts.Clients.Models;
using MediatR;

namespace HillMetrics.MIND.Domain.Contracts.Clients.Commands
{
    public class CreateClientCommand : AuditedCommand<Result<ClientEntity>>
    {
        public SaveClientModel Model { get; }

        public CreateClientCommand(SaveClientModel model)
        {
            Model = model;
        }
    }
}
