using FluentResults;
using HillMetrics.Core.API.Responses;
using HillMetrics.Core.Authentication;
using HillMetrics.Core.Common;
using HillMetrics.Core.Mediator;
using HillMetrics.MIND.API.Contracts.Requests.Clients;
using HillMetrics.MIND.API.Contracts.Responses.Clients;
using HillMetrics.MIND.API.Contracts.Responses.Common;
using HillMetrics.MIND.API.Endpoints;
using HillMetrics.MIND.API.Mappers;
using HillMetrics.MIND.Domain.Contracts.Clients;
using HillMetrics.MIND.Domain.Contracts.Clients.Commands;
using HillMetrics.MIND.Domain.Contracts.Clients.Models;
using HillMetrics.MIND.Domain.Contracts.Clients.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HillMetrics.MIND.API.Controllers
{
    [Route("api/v{v:apiVersion}")]
    public class ClientsController : BaseHillMetricsController
    {
        public ClientsController(IHMediator mediator) : base(mediator)
        {
        }


        [HttpGet(InternalRoutes.Clients.Get)]
        public async Task<ActionResult<GetClientResponse>> GetAsync([FromRoute] int id)
        {
            var query = new GetClientQuery(id);
            var result = await Mediator.Send(query);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            var dto = result.Value.FromDomain();
            return new GetClientResponse(dto);
        }

        [Authorize(Roles = Roles.Mind.ManageClients)]
        [HttpGet(InternalRoutes.Clients.Search)]
        public async Task<ActionResult<ListClientsResponse>> SearchAsync(
            [FromQuery] string? name,
            [FromQuery] string? email,
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 25)
        {
            var query = new SearchClientsQuery(new SearchClientsModel(name, email, pagination: Core.Search.Pagination.New(pageNumber, pageSize)));
            Result<PagedResponse<ClientEntity>> result = await Mediator.Send(query);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            List<ClientDto> dtos = result.Value.Data.FromDomainList();

            return new ListClientsResponse(dtos, result.Value.TotalRecords);
        }

        [Authorize(Roles = Roles.Mind.ManageClients)]
        [HttpPost(InternalRoutes.Clients.Create)]
        public async Task<ActionResult<GetClientResponse>> CreateAsync([FromBody] SaveClientRequest request)
        {
            var command = new CreateClientCommand(new SaveClientModel(request.Name, request.Email, request.IsActive));
            var result = await Mediator.Send(command);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            var dto = result.Value.FromDomain();
            return new GetClientResponse(dto);
        }

        [Authorize(Roles = Roles.Mind.ManageClients)]
        [HttpPut(InternalRoutes.Clients.Update)]
        public async Task<ActionResult<GetClientResponse>> UpdateAsync([FromRoute] int id, [FromBody] SaveClientRequest request)
        {
            var command = new UpdateClientCommand(id, new SaveClientModel(request.Name, request.Email, request.IsActive));
            var result = await Mediator.Send(command);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            var dto = result.Value.FromDomain();
            return new GetClientResponse(dto);
        }

        [Authorize(Roles = Roles.Mind.ManageClients)]
        [HttpDelete(InternalRoutes.Clients.Delete)]
        public async Task<ActionResult<DeletedResponse>> DeleteAsync([FromRoute] int id)
        {
            var command = new DeleteClientCommand(id);
            var result = await Mediator.Send(command);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new DeletedResponse($"Client with id: {id} deleted.");
        }

        [HttpPost(InternalRoutes.Clients.CreateFluxRule)]
        public async Task<ActionResult<GetClientFluxRuleResponse>> CreateFluxRuleAsync(
            [FromRoute] int clientId, 
            [FromBody] SaveClientFluxRuleRequest request)
        {
            var command = new CreateFluxRuleCommand(new SaveClientFluxRuleModel(request.DataPointId, request.PeerGroupId, request.Ranking, request.FluxPriorityList, clientId, request.UseHmDefaultRules));
            var result = await Mediator.Send(command);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            var dto = result.Value.FromDomain();

            return new GetClientFluxRuleResponse(dto);
        }

        [HttpPut(InternalRoutes.Clients.UpdateFluxRule)]
        public async Task<ActionResult<GetClientFluxRuleResponse>> UpdateFluxRuleAsync(
            [FromRoute] int clientId, 
            [FromRoute] int fluxRuleId, 
            [FromBody] SaveClientFluxRuleRequest request)
        {
            var command = new UpdateFluxRuleCommand(fluxRuleId, new SaveClientFluxRuleModel(request.DataPointId, request.PeerGroupId, request.Ranking, request.FluxPriorityList, clientId, request.UseHmDefaultRules));
            var result = await Mediator.Send(command);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            var dto = result.Value.FromDomain();

            return new GetClientFluxRuleResponse(dto);
        }

        [HttpGet(InternalRoutes.Clients.GetFluxRule)]
        public async Task<ActionResult<ListClientFluxRulesResponse>> GetFluxRulesAsync([FromRoute] int clientId)
        {
            var query = new ListClientFluxRulesQuery(clientId);
            var result = await Mediator.Send(query);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            var dtos = result.Value.Data.Select(s => s.FromDomain());

            return new ListClientFluxRulesResponse(dtos, result.Value.TotalRecords);
        }

        [Authorize(Roles = Roles.Mind.ManageClients)]
        [HttpPost(InternalRoutes.Clients.ConstructRefinedDb)]
        public async Task<ActionResult<ConstructRefinedDbResponse>> ConstructRefinedDbAsync([FromRoute]int clientId, CancellationToken cancellationToken)
        {
            var command = new ConstructRefinedDbCommand(clientId);
            var result = await Mediator.Send(command, cancellationToken);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new ConstructRefinedDbResponse("Database for client created");
        }
    }
}