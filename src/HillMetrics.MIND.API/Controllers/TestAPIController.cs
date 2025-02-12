using HillMetrics.Core.API.Responses;
using HillMetrics.MIND.API.Contracts.Responses.Tests;
using HillMetrics.MIND.API.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HillMetrics.MIND.API.Controllers;

[Route("api/v{v:apiVersion}"), AllowAnonymous]
//[EnableRateLimiting("allow5000requestsPerSecond_fixed")]
public class TestAPIController(IMediator mediator) : BaseHillMetricsController(mediator)
{
    [AllowAnonymous]
    [DisableRateLimiting]
    [HttpGet(InternalRoutes.Test.Get)]
    public async Task<ActionResult<TestResponse>> GetAsync()
    {
        return new TestResponse(new TestValue() { TestString = "Hello World", DateTime = DateTime.Now });
    }

    [AllowAnonymous]
    [HttpGet(InternalRoutes.Test.Error)]
    public async Task<ActionResult<TestResponse>> GetAsyncResultError()
    {
        //in case of failure
        return new ErrorApiActionResult(new ErrorApiResponse(new Core.API.Exceptions.ApiException("Bad request"), System.Net.HttpStatusCode.BadRequest));
    }

    //[EnableRateLimiting("singleRequestByUser")]
    [HttpGet("api/test/test123")]
    public async Task<ActionResult<TestResponse>> Test()
    {
        //in case of failure
        return new TestResponse(new TestValue() { TestString = "Hello World", DateTime = DateTime.Now });
    }
}
