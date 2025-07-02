using HillMetrics.Core.AI;
using HillMetrics.Normalized.Domain.Contracts.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.Llm
{
    public class AiLlmEntityDto
    {
        public int Id { get; set; }

        /// <summary>
        /// The name of the LLM model (e.g., OpenAI GPT-4, Azure Document Intelligence, DeepSeek R, mistral, etc)
        /// </summary>
        public string Name { get; set; } = null!;


        /// <summary>
        /// The name of the LLM model Provider
        /// </summary>
        public string Provider { get; set; } = null!;

        /// <summary>
        /// URL to the LLM model's documentation
        /// </summary>
        public string? DocumentationUrl { get; set; }

        public AiProvider HostProvider { get; set; }

        /// <summary>
        /// URL to the logo of the AI model
        /// </summary>
        public string? LogoUrl { get; set; }
        public string? ApiKey { get; set; }
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if the model is currently active
        /// </summary>
        public bool IsActive { get; set; }

        public List<AiLlmHistoryEntityDto>? History { get; set; } = new List<AiLlmHistoryEntityDto>();
        public List<AiLlmTaskTypeSettings> TaskTypeSettings { get; set; } = new List<AiLlmTaskTypeSettings>();
    }
}
