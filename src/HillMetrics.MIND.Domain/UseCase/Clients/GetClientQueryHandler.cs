using FluentResults;
using HillMetrics.Core.Authentication;
using HillMetrics.Core.Errors;
using HillMetrics.Core.Mediator;
using HillMetrics.MIND.Domain.Contracts.Clients;
using HillMetrics.MIND.Domain.Contracts.Clients.Queries;
using HillMetrics.MIND.Domain.Contracts.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.Domain.UseCase.Clients
{
    public class GetClientQueryHandler : Handler<GetClientQueryHandler, ClientEntity, GetClientQuery>
    {
        private readonly IClientService _clientService;
        public GetClientQueryHandler(
            ILogger<GetClientQueryHandler> logger, 
            IClientService clientService) : base(logger)
        {
            _clientService = clientService;
        }

        public override async Task<Result<ClientEntity>> HandleInnerAsync(GetClientQuery request, CancellationToken cancellationToken)
        {
            if (!(request.Audit.User.IsInRole(Roles.Mind.ManageClients) || await _clientService.UserHasClientAccessAsync(request.Audit.User.Id!, request.Id, cancellationToken)))
                return Result.Fail(new ForbidenError("You do not have access to this client."));
            
            return await _clientService.GetAsync(request.Id, cancellationToken);
        }
    }
}
