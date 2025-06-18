using HillMetrics.Core.AI;
using HillMetrics.Core.API.Responses;
using HillMetrics.Core.Mediator;
using HillMetrics.Core.Search;
using HillMetrics.MIND.API.Contracts.Requests.AiPrompts;
using HillMetrics.MIND.API.Contracts.Responses.AiPrompts;
using HillMetrics.MIND.API.Contracts.Responses.Common;
using HillMetrics.MIND.API.Mappers;
using HillMetrics.Normalized.Domain.Contracts.AI.Prompts.Commands;
using HillMetrics.Normalized.Domain.Contracts.AI.Prompts.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HillMetrics.MIND.API.Controllers
{
    public class PromptController : BaseHillMetricsController
    {
        public PromptController(IHMediator mediator) : base(mediator)
        {
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetAiPromptResponse>> GetAsync([FromRoute] int id)
        {
            var query = new GetAiPromptQuery(id);
            var result = await Mediator.Send(query);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            AiPromptDto dto = result.Value.FromDomain();

            return new GetAiPromptResponse(dto);

        }

        [HttpGet("search")]
        public async Task<ActionResult<ListAiPromptsResponse>> SearchPromptsAsync(
            [FromQuery] int? languageId, 
            [FromQuery] PromptTaskType? taskType, 
            [FromQuery] PromptType? promptType, 
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 25)
        {
            var query = new SearchAiPromptsQuery(languageId, promptType, taskType, Pagination.New(pageNumber, pageSize));
            var result = await Mediator.Send(query);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            List<AiPromptDto> dtos = [.. result.Value.Data.Select(s => s.FromDomain())];

            return new ListAiPromptsResponse(dtos, result.Value.TotalRecords);
        }

        [HttpPost]
        public async Task<ActionResult<GetAiPromptResponse>> CreateAsync([FromBody] SaveAiPromptRequest request)
        {
            var command = new CreateAiPromptCommand(new Normalized.Domain.Contracts.AI.Prompts.Models.SaveAiPromptModel()
            {
                LanguageId = request.Prompt.LanguageId,
                TaskType = request.Prompt.TaskType,
                Type = request.Prompt.Type,
                Contents = [.. request.Prompt.Contents.Select(s => new Normalized.Domain.Contracts.AI.Prompts.Models.AiPromptContent(s.Content, s.Type))]
            });

            var result = await Mediator.Send(command);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            AiPromptDto dto = result.Value.FromDomain();

            return new GetAiPromptResponse(dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GetAiPromptResponse>> UpdateAsync([FromRoute] int id, [FromBody] SaveAiPromptRequest request)
        {
            var command = new UpdateAiPromptCommand(
                id, 
                new Normalized.Domain.Contracts.AI.Prompts.Models.SaveAiPromptModel()
            {
                LanguageId = request.Prompt.LanguageId,
                TaskType = request.Prompt.TaskType,
                Type = request.Prompt.Type,
                Contents = [.. request.Prompt.Contents.Select(s => new Normalized.Domain.Contracts.AI.Prompts.Models.AiPromptContent(s.Content, s.Type))]
            });

            var result = await Mediator.Send(command);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            AiPromptDto dto = result.Value.FromDomain();

            return new GetAiPromptResponse(dto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<DeletedResponse>> DeleteAsync([FromRoute]int id)
        {
            var command = new DeleteAiPromptCommand(id);
            var result = await Mediator.Send(command);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new DeletedResponse("Ai prompt deleted.");
        }
    }
}
