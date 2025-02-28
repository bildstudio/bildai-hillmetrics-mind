using FluentResults;
using HillMetrics.Core.AI;
using HillMetrics.Core.AI.Configs;
using HillMetrics.Core.AI.Contracts;
using HillMetrics.Core.Errors;
using HillMetrics.Normalized.Domain.Contracts.AI;
using HillMetrics.Normalized.Domain.Contracts.Repository;
using MassTransit.Configuration;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.Infrastructure.AI
{
    public class LlmServiceFactory : ILlmServiceFactory
    {
        private readonly ILogger<LlmServiceFactory> _logger;
        private readonly IAiModelRepository _aiModelRepository;
        private readonly ILoggerFactory _loggerFactory;
        private readonly AiLlmConfig _aiLlmConfig;

        private Dictionary<int, ILlmService> _services = new Dictionary<int, ILlmService>();

        private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

        public LlmServiceFactory(
            ILogger<LlmServiceFactory> logger,
            IAiModelRepository aiModelRepository,
            ILoggerFactory loggerFactory,
            IOptions<AiLlmConfig> options)
        {
            _logger = logger;
            _aiModelRepository = aiModelRepository;
            _loggerFactory = loggerFactory;
            _aiLlmConfig = options.Value;
        }

        public async Task<Result<ILlmService>> GetServiceAsync(int llmId, CancellationToken cancellationToken)
        {
            await _semaphoreSlim.WaitAsync(cancellationToken);
            try
            {
                if (_services.ContainsKey(llmId))
                    return Result.Ok(_services[llmId]);

                //try to build IllmService

                AiLlmEntity? llmModel = await _aiModelRepository.GetModelByIdAsync(llmId, cancellationToken);
                if (llmModel == null)
                    return Result.Fail(new NotFoundError($"Llm model with id: '{llmId}' not found"));
                
                if(!llmModel.Provider.Contains("deep", StringComparison.OrdinalIgnoreCase) && !llmModel.Provider.Contains("mistral", StringComparison.OrdinalIgnoreCase))
                    return Result.Fail(new NotFoundError($"Llm model for provider : '{llmModel.Provider}' is not supported at the moment"));

                var configModel = _aiLlmConfig.Models.FirstOrDefault(s => s.Provider == llmModel.HostProvider);
                if(configModel == null)
                    return Result.Fail(new ConflictError($"There is no configuration for HostProvider: {llmModel.HostProvider} in appsettings, for llm with id: '{llmModel.Id}', provider: '{llmModel.Provider}', model: '{llmModel.Name}'"));



                IChatClient chatClient = GetChatClient(configModel.Provider, configModel.Endpoint, llmModel.Name, configModel.ApiKey);

                ILlmService llmService = new LlmService(_loggerFactory.CreateLogger<LlmService>(), chatClient);

                _services.Add(llmId, llmService);

                return Result.Ok(llmService);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetServiceAsync error: {ExceptionMessage}", ex.Message);
                return Result.Fail(new Error(ex.Message));
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public IChatClient GetChatClient(AiProvider provider, string endpoint, string modelId, string? apiKey = null)
        {
            switch (provider)
            {
                case AiProvider.Ollama:
                    return new OllamaChatClient(endpoint, modelId);
                case AiProvider.OpenAi:
                    var client = new OpenAI.OpenAIClient(apiKey);
                    return new OpenAIChatClient(client, modelId);
                default:
                    return new OllamaChatClient(endpoint, modelId);
            }
        }
    }
}
