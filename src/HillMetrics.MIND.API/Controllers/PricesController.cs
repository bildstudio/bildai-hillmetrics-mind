using HillMetrics.Core.API.Responses;
using HillMetrics.MIND.API.Contracts.Requests.Prices;
using HillMetrics.Normalized.Domain.Contracts.Market.Cqrs.Price;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HillMetrics.MIND.API.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    public class PricesController : BaseHillMetricsController
    {
        public PricesController(IMediator mediator) : base(mediator)
        {
            
        }

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
    }
}
