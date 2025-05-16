using FluentResults;
using FluentValidation;
using HillMetrics.Core.Mediator;
using HillMetrics.MIND.Domain.Contracts.Clients;
using HillMetrics.MIND.Domain.Contracts.Clients.Commands;
using HillMetrics.MIND.Domain.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace HillMetrics.MIND.Domain.UseCase.Clients
{
    public class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
    {
        public UpdateClientCommandValidator()
        {
            RuleFor(s => s.Model).NotNull().WithMessage("Model is null.");

            When(s => s.Model != null, () => 
            {
                RuleFor(s => s.Model).SetValidator(new SaveClientModelValidator());
                RuleFor(s => s.Id).GreaterThanOrEqualTo(1).WithMessage("Passed id must be positive number.");
            });
        }
    }

    public class UpdateClientCommandHandler : Handler<UpdateClientCommandHandler, ClientEntity, UpdateClientCommand>
    {
        private readonly IClientService _clientService;
        public UpdateClientCommandHandler(
            ILogger<UpdateClientCommandHandler> logger, IClientService clientService)
            : base(logger)
        {
            _clientService = clientService;
        }

        public override Task<Result<ClientEntity>> HandleInnerAsync(UpdateClientCommand request, CancellationToken cancellationToken)
        {
            return _clientService.UpdateAsync(request.Id, request.Model, cancellationToken);
        }
    }
}
