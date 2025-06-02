using FluentResults;
using HillMetrics.Core.AI.Contracts;
using HillMetrics.Core.AI.Models;
using HillMetrics.Core.AI.Configs;
using HillMetrics.Core.AI;
using HillMetrics.Core.Errors;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OllamaSharp;
using Microsoft.Graph.DeviceManagement.ManagedDevices.Item.Retire;

namespace HillMetrics.MIND.Infrastructure.AI
{
    public class ChatService : IChatService
    {
        private readonly ILogger<ChatService> _logger;
        private readonly IChatClient _chatClient;
        private readonly AiLlmConfig _aiLlmConfig;

        // In-memory storage for chats - in a real app, you'd use a database
        private static readonly Dictionary<int, HMChatSession> _chatSessions = new();
        private static int _nextChatId = 1;
        private static int _nextMessageId = 1;
        private static readonly SemaphoreSlim _semaphore = new(1, 1);

        public ChatService(ILogger<ChatService> logger, IChatClient chatClient, IOptions<AiLlmConfig> aiLlmConfig)
        {
            _logger = logger;
            _chatClient = chatClient;
            _aiLlmConfig = aiLlmConfig.Value;
        }

        public IChatClient GetChatClient(string? model = "llama3.2:1b")
        {
            // Utiliser la configuration Ollama par défaut ou la première configuration disponible
            var ollamaConfig = _aiLlmConfig.Models.FirstOrDefault(m => m.Provider == AiProvider.Ollama);

            if (ollamaConfig == null)
                throw new InvalidOperationException("Ollama configuration is not defined");

            //return new OllamaApiClient(ollamaConfig.Endpoint, model!);
            return new OllamaChatClient(ollamaConfig.Endpoint, model!);
        }

        public async Task<Result<int>> CreateNewChatAsync(string title, CancellationToken cancellationToken = default)
        {
            try
            {
                await _semaphore.WaitAsync(cancellationToken);

                var chatSession = new HMChatSession
                {
                    Id = _nextChatId++,
                    Title = title,
                    CreatedAt = DateTime.UtcNow
                };

                _chatSessions[chatSession.Id] = chatSession;

                return Result.Ok(chatSession.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new chat: {Message}", ex.Message);
                return Result.Fail(new Error(ex.Message));
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Result<List<HMChatSession>>> GetAllChatsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _semaphore.WaitAsync(cancellationToken);

                var chats = _chatSessions.Values.ToList();
                return Result.Ok(chats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all chats: {Message}", ex.Message);
                return Result.Fail(new Error(ex.Message));
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Result<List<HMChatMessage>>> GetChatHistoryAsync(int chatId, CancellationToken cancellationToken = default)
        {
            try
            {
                await _semaphore.WaitAsync(cancellationToken);

                if (!_chatSessions.TryGetValue(chatId, out var chatSession))
                    return Result.Fail(new NotFoundError($"Chat with ID {chatId} not found"));

                return Result.Ok(chatSession.Messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chat history: {Message}", ex.Message);
                return Result.Fail(new Error(ex.Message));
            }
            finally
            {
                _semaphore.Release();
            }
        }

        //public async Task<Result<HMChatResponse>> SendMessageAsync(HMChatRequest request, CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        await _semaphore.WaitAsync(cancellationToken);

        //        int chatId = request.ChatId ?? 0;

        //        // Create a new chat if needed
        //        if (chatId == 0)
        //        {
        //            var createResult = await CreateNewChatAsync($"Chat {_nextChatId}", cancellationToken);
        //            if (createResult.IsFailed)
        //                return Result.Fail<HMChatResponse>(createResult.Errors);

        //            chatId = createResult.Value;
        //        }

        //        if (!_chatSessions.TryGetValue(chatId, out var chatSession))
        //            return Result.Fail(new NotFoundError($"Chat with ID {chatId} not found"));

        //        // Add user message to history
        //        var userMessage = new HMChatMessage
        //        {
        //            Id = _nextMessageId++,
        //            ChatId = chatId,
        //            Content = request.Message,
        //            Role = ChatRole.User,
        //            Timestamp = DateTime.UtcNow
        //        };

        //        chatSession.Messages.Add(userMessage);

        //        // Create chat history for LLM context
        //        var chatMessages = chatSession.Messages.Select(m => new Microsoft.Extensions.AI.ChatMessage
        //        {
        //            Contents = null,
        //            Role = m.Role
        //        }).ToList();

        //        // Get response from LLM
        //        var llmResponse = await _chatClient.GetResponseAsync(chatMessages, cancellationToken: cancellationToken);

        //        if (llmResponse == null)
        //            return Result.Fail<HMChatResponse>("No response from LLM");

        //        var responseText = llmResponse.Choices?.FirstOrDefault()?.Text ?? "No response";

        //        // Add assistant response to history
        //        var assistantMessage = new HMChatMessage
        //        {
        //            Id = _nextMessageId++,
        //            ChatId = chatId,
        //            Content = responseText,
        //            Role = ChatRole.Assistant,
        //            Timestamp = DateTime.UtcNow
        //        };

        //        chatSession.Messages.Add(assistantMessage);

        //        return Result.Ok(new HMChatResponse
        //        {
        //            ChatId = chatId,
        //            Response = responseText,
        //            Success = true
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error sending message: {Message}", ex.Message);
        //        return Result.Ok(new HMChatResponse
        //        {
        //            ChatId = request.ChatId ?? 0,
        //            Success = false,
        //            Error = ex.Message
        //        });
        //    }
        //    finally
        //    {
        //        _semaphore.Release();
        //    }
        //}
    }
}