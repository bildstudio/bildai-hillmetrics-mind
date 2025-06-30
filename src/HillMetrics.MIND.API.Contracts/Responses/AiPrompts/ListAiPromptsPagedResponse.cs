namespace HillMetrics.MIND.API.Contracts.Responses.AiPrompts
{
    public class ListAiPromptsPagedResponse : ApiPagedResponseBase<AiPromptDto>
    {
        public ListAiPromptsPagedResponse(IEnumerable<AiPromptDto> data, long totalRecords) : base(data, totalRecords)
        {
        }
    }

    public class ListAiPromptsResponse : ApiResponseBase<List<AiPromptDto>>
    {
        public ListAiPromptsResponse(List<AiPromptDto> data) : base(data)
        {
        }
    }
}
