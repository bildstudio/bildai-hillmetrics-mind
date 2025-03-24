using HillMetrics.Core.API.Responses;
using HillMetrics.Core.Contracts;
using HillMetrics.Core.Messaging.Notification.Realtime;
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
        public TestController(IInfrontService infrontService, ISignalRService signalRService)
        {
            _infrontService = infrontService;
            _signalRService = signalRService;
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
