namespace HillMetrics.MIND.API.Contracts.Responses.AiPrompts
{
    public class ListAiPromptsResponse : ApiPagedResponseBase<AiPromptDto>
    {
        public ListAiPromptsResponse(IEnumerable<AiPromptDto> data, long totalRecords) : base(data, totalRecords)
        {
        }
    }
}
