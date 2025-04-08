using AutoMapper;
using HillMetrics.Core.API.Extensions;
using HillMetrics.Core.API.Responses;
using HillMetrics.MIND.API.Contracts.Requests.Flux;
using HillMetrics.MIND.API.Contracts.Requests.Source;
using HillMetrics.MIND.API.Contracts.Responses.Flux;
using HillMetrics.MIND.API.Contracts.Responses.Source;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux.Cqrs.Create;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux.Cqrs.Get;
using HillMetrics.Normalized.Domain.Contracts.Providing.Source.Cqrs.Create;
using HillMetrics.Normalized.Domain.Contracts.Providing.Source.Cqrs.Get;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HillMetrics.MIND.API.Controllers;

public class SourceController(IMediator mediator, IMapper mapper, ILogger<SourceController> logger) : BaseHillMetricsController(mediator)
{
    /// <summary>
    /// Search for sources following the given criteria
    /// </summary>
    /// <param name="request">Criteria search</param>
    /// <returns></returns>
    [HttpGet("search")]
    public async Task<ActionResult<SourceSearchResponse>> SearchAsync([FromQuery] SourceSearchRequest request)
    {
        var result = await Mediator.Send(mapper.Map<SearchSourceQuery>(request));

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new SourceSearchResponse(mapper.Map<List<SourceSearchDto>>(result.Value.Results));
    }

    /// <summary>
    /// Create a new source provider. A source is link to multiple fluxes.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<SourceResponse>> CreateSourceAsync(SourceCreateRequest request)
    {
        logger.LogInformation("Request for creating a new source");
        var sourceCommand = CreateSourceCommand.Create(request.Name, request.Reliability, request.IsActive);

        var result = await Mediator.Send(sourceCommand);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        logger.LogInformation("New source created");
        return new SourceResponse(result.Value);
    }

    [HttpPut]
    public async Task<ActionResult<SourceResponse>> EditSourceAsync(int sourceId, SourceCreateRequest request)
    {
        var sourceCommand = CreateSourceCommand.Edit(sourceId, request.Name, request.Reliability, request.IsActive);

        var result = await Mediator.Send(sourceCommand);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new SourceResponse(result.Value);
    }
}
