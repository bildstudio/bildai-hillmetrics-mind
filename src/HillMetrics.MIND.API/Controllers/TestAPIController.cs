using HillMetrics.Core.API.Responses;
using HillMetrics.MIND.API.Contracts.Responses.Tests;
using HillMetrics.MIND.API.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HillMetrics.MIND.API.Controllers;

[Route("api/v{v:apiVersion}")]
[EnableRateLimiting("allow5000requestsPerSecond_fixed")]
public class TestAPIController(IMediator mediator) : BaseHillMetricsController(mediator)
{
    [DisableRateLimiting]
    [HttpGet(InternalRoutes.Test.Get)]
    public async Task<ActionResult<TestResponse>> GetAsync()
    {
        return new TestResponse(new TestValue() { TestString = "Hello World", DateTime = DateTime.Now });
    }

    [HttpGet(InternalRoutes.Test.Error)]
    public async Task<ActionResult<TestResponse>> GetAsyncResultError()
    {
        //in case of failure
        return new ErrorAPIResult(new Core.API.Exceptions.ApiException(400, "Bad request", "error details example"));
    }

    [EnableRateLimiting("singleRequestByUser")]
    [HttpGet("api/test/test123")]
    public async Task<ActionResult<TestResponse>> Test()
    {
        //in case of failure
        return new TestResponse(new TestValue() { TestString = "Hello World", DateTime = DateTime.Now });
    }
}
