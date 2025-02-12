using HillMetrics.Core.API.Responses;
using HillMetrics.MIND.API.Contracts.Responses.Tests;
using HillMetrics.MIND.API.Endpoints;
using HillMetrics.MIND.API.Tests;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace HillMetrics.MIND.API.Controllers;

[Route("api/v{v:apiVersion}"), AllowAnonymous]
public class TestApiController: BaseHillMetricsController
{
    public TestApiController(IMediator mediator) : base(mediator)
    {
    }

    [DisableRateLimiting]
    [HttpGet(InternalRoutes.Test.Get)]
    public ActionResult<TestResponse> GetAsync()
    {
        return new TestResponse(new TestValue() { TestString = "Hello World", DateTime = DateTime.Now });
    }

    [HttpGet(InternalRoutes.Test.Error)]
    public ActionResult<TestResponse> GetAsyncResultError()
    {
        //in case of failure
        return new ErrorApiActionResult(new ErrorApiResponse(new Core.API.Exceptions.ApiException("Bad request"), System.Net.HttpStatusCode.BadRequest));
    }
}
