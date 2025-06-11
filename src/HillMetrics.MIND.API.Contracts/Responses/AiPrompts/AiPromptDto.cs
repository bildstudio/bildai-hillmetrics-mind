using HillMetrics.Core.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.API.Contracts.Responses.AiPrompts
{
    public class AiPromptDto
    {
        public int Id { get; set; }
        public PromptTaskType TaskType { get; set; }
        public PromptType Type { get; set; }
        public int LanguageId { get; set; }
        public List<AiPromptContentDto> Contents { get; set; } = [];
    }

    public class AiPromptContentDto
    {
        public AiPromptContentDto(string content, ContentType type)
        {
            Content = content;
            Type = type;
        }

        public string Content { get; set; }
        public ContentType Type { get; set; }
    }
}
