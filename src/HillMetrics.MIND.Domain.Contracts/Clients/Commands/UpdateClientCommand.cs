using FluentResults;
using HillMetrics.Core.Mediator;
using HillMetrics.MIND.Domain.Contracts.Clients.Models;
using MediatR;

namespace HillMetrics.MIND.Domain.Contracts.Clients.Commands
{
    public class UpdateClientCommand : AuditedCommand<Result<ClientEntity>>
    {
        public int Id { get; }
        public SaveClientModel Model { get; }

        public UpdateClientCommand(int id, SaveClientModel model)
        {
            Id = id;
            Model = model;
        }
    }
}
