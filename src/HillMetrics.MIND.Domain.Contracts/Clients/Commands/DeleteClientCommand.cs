using FluentResults;
using HillMetrics.Core.Mediator;
using MediatR;

namespace HillMetrics.MIND.Domain.Contracts.Clients.Commands
{
    public class DeleteClientCommand : AuditedCommand<Result<bool>>
    {
        public int Id { get; }
        public DeleteClientCommand(int id)
        {
            Id = id;
        }
    }
}
