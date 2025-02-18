using AutoMapper;
using FluentResults;
using HillMetrics.Core.API.Responses;
using HillMetrics.MIND.API.Contracts.Responses.Llm;
using HillMetrics.MIND.API.Endpoints;
using HillMetrics.Normalized.Domain.Contracts.AI;
using HillMetrics.Normalized.Domain.Contracts.AI.Cqrs.Get;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HillMetrics.MIND.API.Controllers
{
    [Route("api/v{v:apiVersion}")]
    public class LlmController : BaseHillMetricsController
    {
        private readonly IMapper _mapper;
        public LlmController(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper;
        }

        [HttpGet(InternalRoutes.Llm.GET)]
        public async Task<ActionResult<ListLlmsResponse>> GetAsync([FromQuery] bool? active)
        {
            ListAiLlmEntityQuery query = new ListAiLlmEntityQuery(active);
            Result<List<AiLlmEntity>> result = await Mediator.Send(query);

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            List<AiLlmEntityDto> items = _mapper.Map<List<AiLlmEntityDto>>(result.Value);

            return new ListLlmsResponse(items, items.Count);
        }
    }
}
