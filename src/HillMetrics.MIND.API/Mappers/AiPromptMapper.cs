using HillMetrics.MIND.API.Contracts.Responses.AiPrompts;
using HillMetrics.Normalized.Domain.Contracts.AI.Prompts;

namespace HillMetrics.MIND.API.Mappers
{
    public static class AiPromptMapper
    {
        public static AiPromptDto FromDomain(this AiPrompt aiPrompt)
        {
            return new AiPromptDto
            {
                Id = aiPrompt.Id,
                LanguageId = aiPrompt.LanguageId,
                Type = aiPrompt.Type,
                TaskType = aiPrompt.TaskType,
                Contents = aiPrompt.Content.Select(s => s.FromDomain()).ToList()
            };
        }

        public static AiPromptContentDto FromDomain(this AiPromptContent aiPromptExample)
        {
            return new AiPromptContentDto(aiPromptExample.Content, aiPromptExample.Type);
        }
    }
}
