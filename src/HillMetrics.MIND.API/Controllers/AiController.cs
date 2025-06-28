using HillMetrics.Core.Mediator;
using Microsoft.AspNetCore.Mvc;

namespace HillMetrics.MIND.API.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    public class AiController(
        IHMediator mediator,
        ILogger<AiController> logger) : BaseHillMetricsController(mediator)
    {
       
    }
} 