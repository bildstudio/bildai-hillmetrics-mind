using HillMetrics.Normalized.Domain.Contracts.AI;

namespace HillMetrics.MIND.API.Contracts.Responses.Llm
{
    public class GetLlmTaskTypeSettingsResponse : ApiResponseBase<List<AiLlmTaskTypeSettings>>
    {
        public GetLlmTaskTypeSettingsResponse(List<AiLlmTaskTypeSettings> data) : base(data)
        {
        }
    }
}
