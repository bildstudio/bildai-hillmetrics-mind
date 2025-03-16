using AutoMapper;
using FluentResults;
using HillMetrics.Core.API.Responses;
using HillMetrics.Core.Contracts;
using HillMetrics.MIND.API.Contracts.Requests.Llm;
using HillMetrics.MIND.API.Contracts.Responses.Common;
using HillMetrics.MIND.API.Contracts.Responses.Llm;
using HillMetrics.MIND.API.Endpoints;
using HillMetrics.MIND.API.Mappers;
using HillMetrics.Normalized.Domain.Contracts.AI;
using HillMetrics.Normalized.Domain.Contracts.AI.Commands;
using HillMetrics.Normalized.Domain.Contracts.AI.Models;
using HillMetrics.Normalized.Domain.Contracts.AI.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet(InternalRoutes.Llm.Get)]
        public async Task<ActionResult<ListLlmsResponse>> GetAsync([FromQuery] bool? active)
        {
            ListAiLlmEntityQuery query = new ListAiLlmEntityQuery(active);
            Result<List<AiLlmEntity>> result = await Mediator.Send(query);

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            List<AiLlmEntityDto> items = _mapper.Map<List<AiLlmEntityDto>>(result.Value);

            return new ListLlmsResponse(items, items.Count);
        }

        [HttpPost(InternalRoutes.Llm.Create)]
        public async Task<ActionResult<GetLlmResponse>> CreateAsync([FromBody] CreateLlmRequest request)
        {
            var model = request.ToSaveAiLlmModel();
            var command = new CreateAiLlmModelCommand(model);
            var result = await Mediator.Send(command);

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            var dto = _mapper.Map<AiLlmEntityDto>(result.Value);

            return new GetLlmResponse(dto);
        }

        [HttpPut(InternalRoutes.Llm.Update)]
        public async Task<ActionResult<GetLlmResponse>> UpdateAsync(
            [FromRoute] int id, 
            [FromBody] UpdateLlmRequest request
            )
        {

            var model = request.ToSaveAiLlmModel(id);
            var command = new UpdateAiLlmModelCommand(model);
            var result = await Mediator.Send(command);

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            var dto = _mapper.Map<AiLlmEntityDto>(result.Value);

            return new GetLlmResponse(dto);
        }

        [HttpDelete(InternalRoutes.Llm.Delete)]
        public async Task<ActionResult<DeletedResponse>> DeleteAsync([FromRoute] int id)
        {
            var command = new DeleteAiLlmModelCommand(id);
            var result = await Mediator.Send(command);

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new DeletedResponse($"Llm with id: {id}, deleted");
        }

        //PROMPTS
        //get
        [HttpGet(InternalRoutes.Llm.Prompts.Get)]
        public async Task<ActionResult<GetPromptResponse>> GetAsync([FromRoute]int promptId)
        {
            var query = new GetAiModelPromptQuery(promptId);
            var result = await Mediator.Send(query);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            var dto = _mapper.Map<AiModelPromptDto>(result.Value);
            return new GetPromptResponse(dto);
        }

        //create
        [HttpPost(InternalRoutes.Llm.Prompts.Create)]
        public async Task<ActionResult<GetPromptResponse>> CreateAsync(
            [FromForm] CreatePromptRequest request
            )
        {
            var model = request.ToSaveAiModelPromptModel();
            var command = new CreateAiModelPromptCommand(model);

            var result = await Mediator.Send(command);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            var dto = _mapper.Map<AiModelPromptDto>(result.Value);
            return new GetPromptResponse(dto);
        }

        //update
        [HttpPut(InternalRoutes.Llm.Prompts.Update)]
        public async Task<ActionResult<GetPromptResponse>> UpdateAsync(
            [FromRoute] int promptId, 
            [FromForm] UpdatePromptRequest request)
        {
            var model = request.ToSaveAiModelPromptModel(promptId);
            var command = new UpdateAiModelPromptCommand(model);

            var result = await Mediator.Send(command);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            var dto = _mapper.Map<AiModelPromptDto>(result.Value);

            return new GetPromptResponse(dto);
        }


        //delete
        [HttpDelete(InternalRoutes.Llm.Prompts.Delete)]
        public async Task<ActionResult<DeletedResponse>> DeletePromptAsync([FromRoute] int promptId)
        {
            var command = new DeleteAiModelPromptCommand(promptId);
            var result = await Mediator.Send(command);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new DeletedResponse($"Prompt with id: {promptId} deleted successfully");
        }

        //search
        [HttpGet(InternalRoutes.Llm.Prompts.Search)]
        public async Task<ActionResult<ListPromptsResponse>> SearchAsync(PromptSearchRequest request)
        {
            var model = _mapper.Map<AiModelPromptSearch>(request);
            var query = new SearchPromptsQuery(model);
            var result = await Mediator.Send(query);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            var dtos = _mapper.Map<List<AiModelPromptDto>>(result.Value.Data);

            return new ListPromptsResponse(dtos, result.Value.TotalRecords);
        }

        [HttpPost(InternalRoutes.Llm.DataExtract.Extract)]
        public async Task<ActionResult<ExtractDataLlmResponse>> ExtractDataAsync([FromForm] ExtractDataLlmRequest request)
        {
            ExtractFinancialDataLlmModel model = request.ToExtractFinancialDataLlmModel();
            ExtractFinancialDataLlmCommand command = new ExtractFinancialDataLlmCommand(model);
            
            var result = await Mediator.Send(command);
            if(result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());


            var dtos = _mapper.Map<List<AiModelPromptAnalyzedResultDto>>(result.Value);

            return new ExtractDataLlmResponse(dtos);
        }

        [HttpGet(InternalRoutes.Llm.DataExtract.SearchByPrompt)]
        public async Task<ActionResult<AiModelPromptLlmResultResponse>> SearchByPromptAsync([FromRoute] int promptId)
        {
            var query = new SearchAiModelPromptLlmResultQuery(promptId, null);
            var result = await Mediator.Send(query);

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());


            var dtos = _mapper.Map<List<AiModelPromptLlmResultDto>>(result.Value);

            return new AiModelPromptLlmResultResponse(dtos);
        }

        [HttpGet(InternalRoutes.Llm.DataExtract.SearchByLlm)]
        public async Task<ActionResult<AiModelPromptLlmResultResponse>> SearchByLlmAsync([FromRoute] int llmId)
        {
            var query = new SearchAiModelPromptLlmResultQuery(null, llmId);
            var result = await Mediator.Send(query);

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());


            var dtos = _mapper.Map<List<AiModelPromptLlmResultDto>>(result.Value);

            return new AiModelPromptLlmResultResponse(dtos);
        }

        [HttpGet(InternalRoutes.Llm.DataExtract.Search)]
        public async Task<ActionResult<AiModelPromptLlmResultResponse>> SearchByLlmAsync([FromRoute] int promptId, [FromRoute] int llmId)
        {
            var query = new SearchAiModelPromptLlmResultQuery(promptId, llmId);
            var result = await Mediator.Send(query);

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());


            var dtos = _mapper.Map<List<AiModelPromptLlmResultDto>>(result.Value);

            return new AiModelPromptLlmResultResponse(dtos);
        }
    }
}
