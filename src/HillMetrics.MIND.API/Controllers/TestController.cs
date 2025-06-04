using HillMetrics.Core.API.Responses;
using HillMetrics.Core.Contracts;
using HillMetrics.Core.Flux.Contracts;
using HillMetrics.Core.Mediator;
using HillMetrics.Core.Messaging.Notification.Realtime;
using HillMetrics.Core.Messaging.Services;
using HillMetrics.Normalized.Domain.Contracts.Market.Cqrs.Price;
using HillMetrics.Python.API.SDK.Contracts;
using HillMetrics.Python.API.SDK.Requests.Infront;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace HillMetrics.MIND.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IInfrontService _infrontService;
        private readonly ISignalRService _signalRService;
        private readonly IHMediator _mediator;
        public TestController(IInfrontService infrontService, ISignalRService signalRService, IHMediator mediator)
        {
            _infrontService = infrontService;
            _signalRService = signalRService;
            _mediator = mediator;
        }

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

        [HttpPost("signalr/notifyTopic")]
        public async Task<IActionResult> NotifyTopicAsync([FromBody] NotifyTopicRequest request)
        {
            var notifyEvent = new SignalrPublishEvent(Enum.Parse<NotificationTopic>(request.Topic, true), new DataValue(request.Data, request.Format));

            await _signalRService.PublishAsync(notifyEvent);

            return Ok();
        }

        [HttpPost("signalr/notifyAllUsers")]
        public async Task<IActionResult> NotifyUsersAsync([FromBody] NotifyUsersRequest request)
        {
            var notifyEvent = new SignalrPublishEvent(NotificationTopic.Global, new DataValue(request.Data, request.Format));
            await _signalRService.PublishAsync(notifyEvent);

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("testbench")]
        public async Task<IActionResult> TestBenchs()
        {
            var command = new PriceBenchsCommand(1, 1, new DataProcessing(1, 2, new MemoryStream(), Core.Common.ContentType.Pdf, new Dictionary<string, string>()));

            command.AddItem(new BenchsCommandElement() { Currency = "EUR", Date = DateTime.UtcNow });
            command.AddItem(new BenchsCommandElement() { Currency = "EUR", Date = DateTime.UtcNow });
            command.AddItem(new BenchsCommandElement() { Currency = "EUR", Date = DateTime.UtcNow });
            command.AddItem(new BenchsCommandElement() { Currency = "EUR", Date = DateTime.UtcNow });
            await _mediator.Send(command);

            return Ok();
        }
    }

    public class NotifyTopicRequest
    {
        public string Topic { get; set; }

        //put in separate object, ContentValue  
        public object Data { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DataFormat Format { get; set; }

    }

    public class NotifyUsersRequest
    {
        public object Data { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DataFormat Format { get; set; }
    }
}
