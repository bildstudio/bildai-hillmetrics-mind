using FluentResults;
using FluentValidation;
using HillMetrics.Core.Mediator;
using HillMetrics.MIND.Domain.Contracts.Clients.Commands;
using HillMetrics.MIND.Domain.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace HillMetrics.MIND.Domain.UseCase.Clients
{
    public class DeleteClientCommandValidator : AbstractValidator<DeleteClientCommand>
    {
        public DeleteClientCommandValidator()
        {
            RuleFor(s => s.Id).GreaterThanOrEqualTo(1).WithMessage("Passed id must be positive number.");
        }
    }

    public class DeleteClientCommandHandler : Handler<DeleteClientCommandHandler, bool, DeleteClientCommand>
    {
        private readonly IClientService _clientService;
        public DeleteClientCommandHandler(
            ILogger<DeleteClientCommandHandler> logger, 
            IClientService clientService) 
            : base(logger)
        {
            _clientService = clientService;
        }

        public override Task<Result<bool>> HandleInnerAsync(DeleteClientCommand request, CancellationToken cancellationToken)
        {
            return _clientService.DeleteAsync(request.Id, cancellationToken);
        }
    }
}
