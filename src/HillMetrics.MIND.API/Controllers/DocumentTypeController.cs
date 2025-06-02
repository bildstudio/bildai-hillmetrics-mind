using HillMetrics.Core.API.Responses;
using HillMetrics.Core.Financial;
using HillMetrics.Core.Mediator;
using HillMetrics.Core.Search;
using HillMetrics.MIND.API.Contracts.Requests.AiDataset.DocumentTypes;
using HillMetrics.MIND.API.Contracts.Responses.AiDataset.DocumentTypes;
using HillMetrics.MIND.API.Contracts.Responses.Common;
using HillMetrics.MIND.API.Mappers;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset.Cqrs.DocumentTypes;
using Microsoft.AspNetCore.Mvc;

namespace HillMetrics.MIND.API.Controllers
{
    [Route("api/v{v:apiVersion}/document-type")]
    public class DocumentTypeController : BaseHillMetricsController
    {
        public DocumentTypeController(IHMediator mediator) : base(mediator)
        {
        }

        //get
        [HttpGet("{id}")]
        public async Task<ActionResult<GetDocumentTypeResponse>> GetAsync([FromRoute] int id)
        {
            var query = new GetDocumentTypeQuery(id);
            var result = await Mediator.Send(query);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            DocumentTypeDto dto = result.Value.FromDomain();

            return new GetDocumentTypeResponse(dto);

        }
        //search
        [HttpGet("search")]
        public async Task<ActionResult<ListDocumentTypesResponse>> ListAsync(
            [FromQuery] SearchDocumentTypeRequest request)
        {
            var query = new SearchDocumentTypesQuery(request.Name, request.FinancialType, request.Pagination);
            var result = await Mediator.Send(query);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            List<DocumentTypeDto> dtos = result.Value.Data.FromDomains();

            return new ListDocumentTypesResponse(dtos, result.Value.TotalRecords);
        }

        //post
        [HttpPost]
        public async Task<ActionResult<GetDocumentTypeResponse>> CreateAsync([FromBody] SaveDocumentTypeRequest request)
        {
            var command = new CreateDocumentTypeCommand(new SaveDocumentTypeModel(request.Name, request.FinancialType));
            var result = await Mediator.Send(command);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            DocumentTypeDto dto = result.Value.FromDomain();

            return new GetDocumentTypeResponse(dto);
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<GetDocumentTypeResponse>> UpdateAsync([FromRoute] int id, [FromBody] SaveDocumentTypeRequest request)
        {
            var command = new UpdateDocumentTypeCommand(
                id,
                new SaveDocumentTypeModel(request.Name, request.FinancialType));

            var result = await Mediator.Send(command);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            DocumentTypeDto dto = result.Value.FromDomain();

            return new GetDocumentTypeResponse(dto);
        }

        
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeletedResponse>> DeleteAsync([FromRoute] int id)
        {
            var command = new DeleteDocumentTypeCommand(id);

            var result = await Mediator.Send(command);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new DeletedResponse($"DocumentType with id : {id}, deleted.");
        }
    }
}
