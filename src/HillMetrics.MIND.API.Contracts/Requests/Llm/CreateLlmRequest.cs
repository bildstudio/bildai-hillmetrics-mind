using HillMetrics.Core.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.Llm
{
    public class CreateLlmRequest
    {
        public required string Name { get; set; }


        /// <summary>
        /// The name of the LLM model Provider
        /// </summary>
        public required string Provider { get; set; }

        /// <summary>
        /// URL to the LLM model's documentation
        /// </summary>
        public string? DocumentationUrl { get; set; }

        public required AiProvider HostProvider { get; set; }

        /// <summary>
        /// URL to the logo of the AI model
        /// </summary>
        public string? LogoUrl { get; set; }
    }

    public class UpdateLlmRequest : CreateLlmRequest
    {
        
    }
}
