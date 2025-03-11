using HillMetrics.Core.AI;
using System.Text.Json.Serialization;

namespace HillMetrics.MIND.API.Contracts.Responses.Llm
{
    public class AiModelPromptAnalyzedResultDto
    {
        public int PromptId { get; set; }
        public string? Llm { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LlmProcessingStatus Status { get; set; }
        public string? Result { get; set; } = string.Empty;
        public string? Error { get; set; } = string.Empty;
    }
}
