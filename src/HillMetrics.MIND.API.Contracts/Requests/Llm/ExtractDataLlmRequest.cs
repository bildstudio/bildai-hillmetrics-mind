using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.Llm
{
    public class ExtractDataLlmRequest
    {
        public int PromptId { get; set; }
        public required List<int> AiModelsIds { get; set; } = [];
        public required IFormFile File { get; set; }
    }
}
