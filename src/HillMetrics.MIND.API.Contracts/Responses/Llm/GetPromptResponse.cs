namespace HillMetrics.MIND.API.Contracts.Responses.Llm
{
    public class GetPromptResponse : ApiResponseBase<AiModelPromptDto>
    {
        public GetPromptResponse(AiModelPromptDto data) : base(data)
        {
            
        }
    }
}
