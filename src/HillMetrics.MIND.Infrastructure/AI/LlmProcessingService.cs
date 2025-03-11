using FluentResults;
using HillMetrics.Core.AI;
using HillMetrics.Core.AI.Contracts;
using HillMetrics.Core.AI.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace HillMetrics.MIND.Infrastructure.AI
{
    public class LlmProcessingService : ILlmProcessingService
    {
        private readonly ILogger<LlmProcessingService> _logger;
        private readonly IChatClient _chatClient;

        public LlmProcessingService(ILogger<LlmProcessingService> logger, IChatClient chatClient)
        {
            _logger = logger;
            _chatClient = chatClient;
        }

        public async Task<Result<LlmProcessResponse>> AnalyzeFileAsync(LlmAnalyzeFileModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                List<ChatMessage> chatMessages = [
                    new ChatMessage()
                    {
                        Text = model.PromptText,
                        Role = ChatRole.User
                    },
                    new ChatMessage()
                    {
                        Text = $"\n #### **Input JSON to be analyzed:**: \n" +
                                    $"```json" +
                                    $"\n{model.FileContentJson}\n" +
                                    $"```",
                        Role = ChatRole.User
                    }
                ];

                var chatOptions = new ChatOptions
                {
                    ResponseFormat = ParseResponseFormat(model.ResponseFormat)
                };

                var llmResult = await _chatClient.GetResponseAsync(chatMessages, options: chatOptions, cancellationToken: cancellationToken);

                if (llmResult == null)
                    return Result.Fail("no response");

                LlmProcessResponse response = new LlmProcessResponse(llmResult.Choices?.FirstOrDefault()?.Text);

                return Result.Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AnalyzeFileAsync error: {ExceptionMessage}", ex.Message);
                return Result.Fail<LlmProcessResponse>(ex.Message);
            }
        }

        public async Task<Result<GenericLlmProcessResponse<T>>> AnalyzeFileAsync<T>(LlmAnalyzeFileModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                List<ChatMessage> chatMessages = [
                    new ChatMessage()
                    {
                        Text = model.PromptText,
                        Role = ChatRole.User
                    },
                    new ChatMessage()
                    {
                        Text = $"\n #### **Input JSON to be analyzed:**: \n" +
                                    $"```json" +
                                    $"\n{model.FileContentJson}\n" +
                                    $"```",
                        Role = ChatRole.User
                    }
                ];

                var chatOptions = new ChatOptions
                {
                    ResponseFormat = ParseResponseFormat(model.ResponseFormat)
                };

                var llmResult = await _chatClient.GetResponseAsync<T>(chatMessages, options: chatOptions, cancellationToken: cancellationToken);

                if (llmResult == null)
                    return Result.Fail("no response");

                GenericLlmProcessResponse<T> response = new GenericLlmProcessResponse<T>(llmResult.Result);

                return Result.Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AnalyzeFileAsync error: {ExceptionMessage}", ex.Message);
                return Result.Fail<GenericLlmProcessResponse<T>>(ex.Message);
            }
        }

        private static ChatResponseFormat ParseResponseFormat(AiResponseFormat responseFormat)
        {
            switch (responseFormat)
            {
                case AiResponseFormat.Text:
                    return ChatResponseFormat.Text;
                case AiResponseFormat.Json:
                    return ChatResponseFormat.Json;
                default:
                    return ChatResponseFormat.Text;
            }
        }
    }
}
