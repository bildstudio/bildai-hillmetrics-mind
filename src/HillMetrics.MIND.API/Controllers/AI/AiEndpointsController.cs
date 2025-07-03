using HillMetrics.Core.API.Responses;
using HillMetrics.Core.Mediator;
using HillMetrics.Core.Search;
using HillMetrics.MIND.API.Contracts.Requests.AiEndpoints;
using HillMetrics.MIND.API.Contracts.Responses.AiEndpoints;
using HillMetrics.MIND.API.Contracts.Responses.Common;
using HillMetrics.Normalized.Domain.Contracts.AI.Endpoints.Commands;
using HillMetrics.Normalized.Domain.Contracts.AI.Endpoints.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HillMetrics.MIND.API.Controllers.AI
{
    [Route("api/v{v:apiVersion}/ai/internal/endpoints")]
    public class AiEndpointsController : BaseHillMetricsController
    {
        public AiEndpointsController(IHMediator mediator) : base(mediator)
        {
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetAiEndpointResponse>> GetAiEndpointAsync(
            [FromRoute] int id, 
            CancellationToken cancellationToken)
        {
            GetAiEndpointQuery query = new GetAiEndpointQuery(id);
            var result = await Mediator.Send(query, cancellationToken);

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new GetAiEndpointResponse(result.Value);
        }

        [HttpGet("search")]
        public async Task<ActionResult<ListAiEndpointsResponse>> SearchAiEndpointAsync(
            [FromQuery] string? searchTerm, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 25,
            CancellationToken cancellationToken = default)
        {
            SearchAiEndpointsQuery query = new SearchAiEndpointsQuery(searchTerm, Pagination.New(pageNumber, pageSize));
            var result = await Mediator.Send(query, cancellationToken);

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new ListAiEndpointsResponse(result.Value.Data, result.Value.TotalRecords);
        }

        [HttpPost]
        public async Task<ActionResult<GetAiEndpointResponse>> CreateAiEndpointAsync(
            [FromBody] SaveAiEndpointRequest request, 
            CancellationToken cancellationToken)
        {
            CreateAiEndpointCommand command = new CreateAiEndpointCommand(request.Endpoint);
            var result = await Mediator.Send(command, cancellationToken);

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new GetAiEndpointResponse(result.Value);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GetAiEndpointResponse>> UpdateAiEndpointAsync(
            [FromRoute] int id,
            [FromBody] SaveAiEndpointRequest request, 
            CancellationToken cancellationToken)
        {
            UpdateAiEndpointCommand command = new UpdateAiEndpointCommand(id, request.Endpoint);
            var result = await Mediator.Send(command, cancellationToken);

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new GetAiEndpointResponse(result.Value);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<DeletedResponse>> DeleteAiEndpointAsync(
            [FromRoute] int id,
            CancellationToken cancellationToken)
        {
            DeleteAiEndpointCommand command = new DeleteAiEndpointCommand(id);
            var result = await Mediator.Send(command, cancellationToken);

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new DeletedResponse("AI endpoint deleted.");
        }
    }
}
