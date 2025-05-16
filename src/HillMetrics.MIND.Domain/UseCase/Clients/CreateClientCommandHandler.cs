using FluentResults;
using FluentValidation;
using HillMetrics.Core.Mediator;
using HillMetrics.MIND.Domain.Contracts.Clients;
using HillMetrics.MIND.Domain.Contracts.Clients.Commands;
using HillMetrics.MIND.Domain.Contracts.Clients.Models;
using HillMetrics.MIND.Domain.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace HillMetrics.MIND.Domain.UseCase.Clients
{
    public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
    {
        public CreateClientCommandValidator()
        {
            RuleFor(s => s.Model).NotNull().WithMessage("Model is null.");
            
            RuleFor(s => s.Model).SetValidator(new SaveClientModelValidator()).When(s => s.Model != null);
        }
    }

    public class SaveClientModelValidator : AbstractValidator<SaveClientModel>
    {
        public SaveClientModelValidator()
        {
            RuleFor(s => s.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(s => s.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.");
        }
    }

    public class CreateClientCommandHandler : Handler<CreateClientCommandHandler, ClientEntity, CreateClientCommand>
    {
        private readonly IClientService _clientService;

        public CreateClientCommandHandler(
            ILogger<CreateClientCommandHandler> logger,
            IClientService clientService)
            : base(logger)
        {
            _clientService = clientService;
        }

        public override Task<Result<ClientEntity>> HandleInnerAsync(CreateClientCommand request, CancellationToken cancellationToken)
        {
            return _clientService.CreateAsync(request.Model, cancellationToken);
        }
    }
}
