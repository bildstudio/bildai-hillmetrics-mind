using HillMetrics.Core.API.Responses;
using HillMetrics.Python.API.SDK.Contracts;
using HillMetrics.Python.API.SDK.Requests.Infront;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HillMetrics.MIND.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IInfrontService _infrontService;
        public TestController(IInfrontService infrontService)
        {
            _infrontService = infrontService;
        }

        [AllowAnonymous]
        [HttpPost("python-api/infront")]
        public async Task<ActionResult<Python.API.SDK.Responses.Infront.FetchDataResponse>> InFrontFetchDataAsync(
            [FromBody] FetchDataRequest request
            )
        {
            //FetchDataRequest request = new FetchDataRequest(ticker, startDate, endDate);

            var result = await _infrontService.FetchDataAsync(request);

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return result.Value;
        }
    }
}
