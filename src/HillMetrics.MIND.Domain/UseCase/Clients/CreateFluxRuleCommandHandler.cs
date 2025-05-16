using FluentResults;
using FluentValidation;
using HillMetrics.Core.Authentication;
using HillMetrics.Core.Errors;
using HillMetrics.Core.Mediator;
using HillMetrics.Core.Storage.Database.Contracts;
using HillMetrics.MIND.Domain.Contracts.Clients;
using HillMetrics.MIND.Domain.Contracts.Clients.Commands;
using HillMetrics.MIND.Domain.Contracts.Clients.Models;
using HillMetrics.MIND.Domain.Contracts.Services;
using HillMetrics.Normalized.Infrastructure.Contracts.Database.Entity;
using HillMetrics.Normalized.Infrastructure.Contracts.Database.Entity.Flux;
using HillMetrics.Normalized.Infrastructure.Database.Database;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using System.Text;

namespace HillMetrics.MIND.Domain.UseCase.Clients
{
    public class CreateFluxRuleCommandValidator : AbstractValidator<CreateFluxRuleCommand>
    {
        public CreateFluxRuleCommandValidator()
        {
            RuleFor(s => s.Model).NotNull().WithMessage("Passed model is null.");
            When(s => s.Model != null, () =>
            {
                RuleFor(s => s.Model).SetValidator(new SaveClientFluxRuleModelValidator());
            });
        }
    }

    public class SaveClientFluxRuleModelValidator : AbstractValidator<SaveClientFluxRuleModel>
    {
        public SaveClientFluxRuleModelValidator()
        {
            RuleFor(s => s.ClientId).GreaterThan(0).WithMessage("ClientId must be positive number");
            RuleFor(s => s.PeerGroupId).GreaterThan(0).WithMessage("PeerGroupId must be positive number");
            RuleFor(s => s.Ranking).GreaterThanOrEqualTo(0).WithMessage("PeerGroupId must be positive number or 0");
            RuleFor(s => s.DataPointId).GreaterThan(0).WithMessage("DataPointId must be positive number");
            RuleFor(s => s.FluxPriorityList).NotEmpty().WithMessage("FluxPriorityList is required. Can not be empty.");
        }
    }

    public class CreateFluxRuleCommandHandler : Handler<CreateFluxRuleCommandHandler, ClientFluxRule, CreateFluxRuleCommand>
    {
        private readonly IClientService _clientService;
        private readonly IReadOnlyRepository<FinancialDataPointEntity> _dataPointRepo;
        private readonly IReadOnlyRepository<FluxEntity> _fluxEntityRepo;
        public CreateFluxRuleCommandHandler(
            ILogger<CreateFluxRuleCommandHandler> logger, 
            IClientService clientService,
            IReadonlyUnitOfWork<FinancialNormalizedContext> unitOfWork) 
            : base(logger)
        {
            _clientService = clientService;
            _dataPointRepo = unitOfWork.GetReadonlyRepository<FinancialDataPointEntity>();
            _fluxEntityRepo = unitOfWork.GetReadonlyRepository<FluxEntity>();
        }

        public override async Task<Result<ClientFluxRule>> HandleInnerAsync(CreateFluxRuleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (!(request.Audit.User.IsInRole(Roles.Mind.ManageClients) || await _clientService.UserHasClientAccessAsync(request.Audit.User.Id!, request.Model.ClientId, cancellationToken)))
                    return Result.Fail(new ForbidenError("You do not have access to this client."));

                //check first if refined db contains elements
                FinancialDataPointEntity financialDataPoint = await _dataPointRepo.SingleOrDefaultAsync(s => s.Id == request.Model.DataPointId, cancellationToken: cancellationToken);
                if (financialDataPoint == null)
                    return Result.Fail(new NotFoundError($"Data point with id: {request.Model.DataPointId} not found"));

                List<FluxEntity> fluxEntities = await _fluxEntityRepo.ToListAsync(s => request.Model.FluxPriorityList.Contains(s.Id), cancellationToken: cancellationToken);
                StringBuilder fluxInErrorBuilder = new StringBuilder("");
                foreach (var fluxId in request.Model.FluxPriorityList)
                {
                    if(fluxEntities.FirstOrDefault(s => s.Id == fluxId) == null)
                    {
                        fluxInErrorBuilder.Append($"Flux with id: {fluxId} not found. {Environment.NewLine}");
                    }
                }

                if(fluxInErrorBuilder.Length > 0)
                    return Result.Fail(new NotFoundError($"Flux entities not found:[{fluxInErrorBuilder.ToString()}]"));

                Result<ClientFluxRule> clientFluxRuleResult = await _clientService.CreateFluxRuleAsync(request.Model, cancellationToken);
                if (clientFluxRuleResult.IsFailed)
                    return clientFluxRuleResult.ToResult<ClientFluxRule>();

                return clientFluxRuleResult.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "HandleInnerAsync error: {ExceptionMessage}", ex.Message);
                return Result.Fail(new InternalServerError(ex.Message));
            }
        }
    }

}
