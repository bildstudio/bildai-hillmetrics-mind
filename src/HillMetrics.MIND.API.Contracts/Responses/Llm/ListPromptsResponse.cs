namespace HillMetrics.MIND.API.Contracts.Responses.Llm
{
    public class ListPromptsResponse : CustomMindPagedApiResponseBase<AiModelPromptDto>
    {
        public ListPromptsResponse(List<AiModelPromptDto> data, int totalRecords) : base(data, totalRecords)
        {

        }
    }
}
