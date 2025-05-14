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
using HillMetrics.Normalized.Domain.Contracts.Clients;
using HillMetrics.Normalized.Domain.Contracts.Clients.Commands;
using HillMetrics.Normalized.Domain.Contracts.Clients.Models;
using HillMetrics.Normalized.Domain.Contracts.Clients.Queries;
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
            var command = new CreateClientCommand(new SaveClientModel(request.Name, request.Email));
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
            var command = new UpdateClientCommand(id, new SaveClientModel(request.Name, request.Email));
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
    }
}
