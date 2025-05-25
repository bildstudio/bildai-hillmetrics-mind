using FluentResults;
using HillMetrics.Core.AI.Contracts;
using HillMetrics.Core.AI.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace HillMetrics.MIND.Infrastructure.AI
{
    public class DisabledChatService : IChatService
    {
        private readonly ILogger<DisabledChatService> _logger;
        private const string DisabledMessage = "Le service de chat AI est désactivé dans cet environnement.";

        public DisabledChatService(ILogger<DisabledChatService> logger)
        {
            _logger = logger;
            _logger.LogInformation("Le service de chat AI est désactivé.");
        }

        public IChatClient GetChatClient()
        {
            _logger.LogWarning("Tentative d'utilisation du service de chat AI désactivé: GetChatClient");
            // Retourner un client par défaut même si désactivé
            return new OllamaChatClient("http://localhost:11434", "llama3.2:1b");
        }

        public Task<Result<int>> CreateNewChatAsync(string title, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning("Tentative d'utilisation du service de chat AI désactivé: CreateNewChatAsync");
            return Task.FromResult(Result.Fail<int>(DisabledMessage));
        }

        public Task<Result<List<HMChatSession>>> GetAllChatsAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogWarning("Tentative d'utilisation du service de chat AI désactivé: GetAllChatsAsync");
            return Task.FromResult(Result.Ok(new List<HMChatSession>()));
        }

        public Task<Result<List<HMChatMessage>>> GetChatHistoryAsync(int chatId, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning("Tentative d'utilisation du service de chat AI désactivé: GetChatHistoryAsync");
            return Task.FromResult(Result.Ok(new List<HMChatMessage>()));
        }

        public Task<Result<HMChatResponse>> SendMessageAsync(HMChatRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogWarning("Tentative d'utilisation du service de chat AI désactivé: SendMessageAsync");
            return Task.FromResult(Result.Fail<HMChatResponse>(DisabledMessage));
        }
    }
}