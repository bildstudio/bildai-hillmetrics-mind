using HillMetrics.MIND.API.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HillMetrics.MIND.API.Controllers;

//[Authorize]
[ApiController]
[Route($"{InternalRoutes.Prefix}")]
public class BaseHillMetricsController : ControllerBase
{
    protected readonly IMediator Mediator;
    public BaseHillMetricsController(IMediator mediator)
    {
        Mediator = mediator;
    }
}
