namespace HillMetrics.MIND.API.Contracts.Responses.Llm
{
    public class AiModelPromptLlmResultResponse : ApiResponseBase<List<AiModelPromptLlmResultDto>>
    {
        public AiModelPromptLlmResultResponse(List<AiModelPromptLlmResultDto> data) : base(data)
        {
            
        }
    }
}