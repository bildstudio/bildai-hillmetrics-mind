using AutoMapper;
using HillMetrics.Core.API.Responses;
using HillMetrics.Core.Workflow;
using HillMetrics.MIND.API.Contracts.Requests.Prices;
using HillMetrics.MIND.API.Contracts.Responses;
using HillMetrics.MIND.API.Contracts.Responses.Flux;
using HillMetrics.MIND.API.Contracts.Responses.Prices;
using HillMetrics.Normalized.Domain.Contracts.Market.Cqrs.Price;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HillMetrics.MIND.API.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    public class PricesController(IMediator mediator, IMapper mapper, ILogger<PricesController> logger) : BaseHillMetricsController(mediator)
    {
        [HttpPost("update")]
        public async Task<ActionResult<bool>> UpdatePriceAsync([FromBody] UpdatePriceRequest request)
        {
            UpdatePriceEntityModel model = new UpdatePriceEntityModel
            {
                CurrencyCode = request.CurrencyCode,
                Date = request.Date,
                FinancialId = request.FinancialId,
                FluxId = request.FluxId,
                FinancialType = request.FinancialType,
                Properties = request.Properties
            };

            var command = new UpdatePriceEntityCommand(model);
            
            var result = await Mediator.Send(command);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return true;
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedApiResponseBase<SearchPricesResponse>>> SearchPricesAsync([FromQuery] SearchPricesRequest request)
        {
            var model = new SearchPriceEntityModel()
            {
                Code = request.Code,
                CurrencyCode = request.CurrencyCode,
                FinancialId = request.FinancialId.IsSet() ? request.FinancialId : null,
                FinancialType = request.FinancialType,
                FluxId = request.FluxId.IsSet() ? request.FluxId : null,
                FluxProcessingContentId = request.FluxProcessingContentId.IsSet() ? request.FluxProcessingContentId : null,
                From = request.From.IsSet() ? request.From : null,
                To = request.To.IsSet() ? request.To : null
            };

            var query = new SearchPriceEntityQuery(model, request.Pagination, request.Sorting);
            var result = await Mediator.Send(query);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            //return new PagedApiResponseBase<FluxSearchResponse>(mapper.Map<List<FluxSearchResponse>>(result.Value.Results), result.Value.NbTotalRows);
            return new PagedApiResponseBase<SearchPricesResponse>(mapper.Map<List<SearchPricesResponse>>(result.Value.Data), result.Value.TotalRecords);
        }
    }
}
