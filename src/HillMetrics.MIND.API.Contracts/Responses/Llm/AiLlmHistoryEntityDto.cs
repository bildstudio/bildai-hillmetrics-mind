namespace HillMetrics.MIND.API.Contracts.Responses.Llm
{
    public class AiLlmHistoryEntityDto
    {
        public int Id { get; set; }

        /// <summary>
        /// The request prompt sent to the AI model
        /// </summary>
        public string Prompt { get; set; } = null!;

        /// <summary>
        /// The response received from the AI model
        /// </summary>
        public string? Response { get; set; }

        /// <summary>
        /// The context in which the LLM was used
        /// </summary>
        public string? Context { get; set; }
    }
}
