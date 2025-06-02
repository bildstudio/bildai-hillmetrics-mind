using HillMetrics.Core.API.Responses;
using HillMetrics.Core.Mediator;
using HillMetrics.MIND.API.Contracts.Requests.Languages;
using HillMetrics.MIND.API.Contracts.Responses.Common;
using HillMetrics.MIND.API.Contracts.Responses.Languages;
using HillMetrics.MIND.API.Mappers;
using HillMetrics.Normalized.Domain.Contracts.Languages.Commands;
using HillMetrics.Normalized.Domain.Contracts.Languages.Models;
using HillMetrics.Normalized.Domain.Contracts.Languages.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HillMetrics.MIND.API.Controllers
{
    public class LanguageController : BaseHillMetricsController
    {
        public LanguageController(IHMediator mediator) : base(mediator)
        {
        }

        //get
        [HttpGet("{id}")]
        public async Task<ActionResult<GetLanguageResponse>> GetAsync([FromRoute] int id)
        {
            var query = new GetLanguageQuery(id);
            var result = await Mediator.Send(query);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            LanguageDto dto = result.Value.FromDomain();

            return new GetLanguageResponse(dto);

        }
        //list
        [HttpGet("list")]
        public async Task<ActionResult<ListLanguageResponse>> ListAsync()
        {
            var query = new ListLanguagesQuery();
            var result = await Mediator.Send(query);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            List<LanguageDto> dtos = result.Value.Data.FromDomains();

            return new ListLanguageResponse(dtos, result.Value.TotalRecords);
        }

        //post
        [HttpPost]
        public async Task<ActionResult<GetLanguageResponse>> CreateAsync([FromBody] SaveLanguageRequest request)
        {
            var command = new CreateLanguageCommand(new SaveLanguageModel(request.Name, request.TwoLetterCode, request.IsActive));
            var result = await Mediator.Send(command);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            LanguageDto dto = result.Value.FromDomain();

            return new GetLanguageResponse(dto);
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<GetLanguageResponse>> UpdateAsync([FromRoute] int id, [FromBody] SaveLanguageRequest request)
        {
            var command = new UpdateLanguageCommand(
                id, 
                new SaveLanguageModel(request.Name, request.TwoLetterCode, request.IsActive));

            var result = await Mediator.Send(command);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            LanguageDto dto = result.Value.FromDomain();

            return new GetLanguageResponse(dto);
        }

        
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeletedResponse>> DeleteAsync([FromRoute] int id)
        {
            var command = new DeleteLanguageCommand(id);

            var result = await Mediator.Send(command);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new DeletedResponse($"Language with id : {id}, deleted.");
        }
    }
}
