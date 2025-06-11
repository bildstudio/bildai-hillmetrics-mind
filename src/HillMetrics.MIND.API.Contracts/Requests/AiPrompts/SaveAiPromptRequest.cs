using HillMetrics.MIND.API.Contracts.Responses.AiPrompts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Requests.AiPrompts
{
    public class SaveAiPromptRequest
    {
        public AiPromptDto Prompt { get; set; } = null!;
    }
}
