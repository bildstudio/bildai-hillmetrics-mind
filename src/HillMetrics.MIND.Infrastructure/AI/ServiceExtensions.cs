using HillMetrics.Core.AI.Contracts;
using HillMetrics.Core.AI.Configs;
using HillMetrics.Core.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HillMetrics.MIND.Infrastructure.AI
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddAiChatServices(this IServiceCollection services, bool enabled, IConfiguration? configuration = null)
        {
            if (enabled)
            {
                // Enregistrer IChatClient avec configuration simplifiée
                services.AddSingleton<IChatClient>(provider =>
                {
                    var logger = provider.GetRequiredService<ILogger<OllamaChatClient>>();
                    var aiConfig = provider.GetRequiredService<IOptions<AiLlmConfig>>().Value;

                    // Utiliser la première configuration Ollama disponible
                    var ollamaConfig = aiConfig.Models.FirstOrDefault(m => m.Provider == AiProvider.Ollama);

                    if (ollamaConfig != null)
                    {
                        logger.LogInformation("Using Ollama endpoint: {Endpoint}", ollamaConfig.Endpoint);
                        return new OllamaChatClient(ollamaConfig.Endpoint, "llama3.2:1b");
                    }

                    // Fallback vers la configuration par défaut
                    var defaultEndpoint = "http://localhost:11434";
                    logger.LogWarning("No Ollama configuration found, using default endpoint: {Endpoint}", defaultEndpoint);
                    return new OllamaChatClient(defaultEndpoint, "llama3.2:1b");
                });

                services.AddScoped<IChatService, ChatService>();
            }
            else
            {
                services.AddScoped<IChatService, DisabledChatService>();
            }

            return services;
        }
    }
}