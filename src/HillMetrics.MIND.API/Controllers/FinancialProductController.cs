using AutoMapper;
using HillMetrics.Core.API.Extensions;
using HillMetrics.Core.API.Responses;
using HillMetrics.MIND.API.Contracts.Responses.Flux;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux.Cqrs.Get;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HillMetrics.MIND.API.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]"), AllowAnonymous]
    public class FinancialProductController(IMediator mediator, IMapper mapper) : BaseHillMetricsController(mediator)
    {
        //[HttpGet("{id}")]
        //public async Task<ActionResult<FluxResponse>> GetAsync(int id)
        //{
        //    var result = await Mediator.Send(new FluxQuery() { FluxId = id });

        //    if (result.IsFailed)
        //        return new ErrorAPIResult(result.Errors.ToApiException());

        //    return new FluxResponse(mapper.Map<FluxResponse>(result.Value));
        //}

    }
}
