using HillMetrics.Core.API.Responses;
using HillMetrics.MIND.API.Contracts.Responses.Tests;
using HillMetrics.MIND.API.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HillMetrics.MIND.API.Controllers;

public class TestAPIController(IMediator mediator) : BaseHillMetricsController(mediator)
{
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
}
