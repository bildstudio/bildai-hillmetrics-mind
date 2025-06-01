using FluentResults;
using HillMetrics.Core.AI.Contracts;
using HillMetrics.Core.Errors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HillMetrics.MIND.Infrastructure.AI
{
    public class OllamaModelService : IOllamaModelService
    {
        private readonly ILogger<OllamaModelService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _ollamaEndpoint;

        public OllamaModelService(ILogger<OllamaModelService> logger, HttpClient httpClient, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _ollamaEndpoint = configuration.GetValue<string>("AI:Ollama:Endpoint") ?? "http://localhost:11434";
        }

        public async Task<Result<List<OllamaModelInfo>>> GetAvailableModelsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Fetching available Ollama models from {Endpoint}", _ollamaEndpoint);

                var response = await _httpClient.GetAsync($"{_ollamaEndpoint}/api/tags", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = $"Failed to fetch models from Ollama. Status: {response.StatusCode}";
                    _logger.LogError(errorMessage);
                    return Result.Fail(new Error(errorMessage));
                }

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var ollamaResponse = JsonSerializer.Deserialize<OllamaTagsResponse>(content, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });

                if (ollamaResponse?.Models == null)
                {
                    _logger.LogWarning("No models found in Ollama response");
                    return Result.Ok(new List<OllamaModelInfo>());
                }

                var models = ollamaResponse.Models.Select(m => new OllamaModelInfo
                {
                    Name = m.Name ?? string.Empty,
                    Size = m.Size,
                    ModifiedAt = m.ModifiedAt,
                    Family = m.Details?.Family ?? string.Empty,
                    Format = m.Details?.Format ?? Array.Empty<string>(),
                    Families = m.Details?.Families ?? Array.Empty<string>(),
                    ParameterSize = m.Details?.ParameterSize ?? 0,
                    QuantizationLevel = m.Details?.QuantizationLevel ?? 0
                }).ToList();

                _logger.LogInformation("Found {Count} models in Ollama", models.Count);
                return Result.Ok(models);
            }
            catch (HttpRequestException ex)
            {
                var errorMessage = $"Network error connecting to Ollama at {_ollamaEndpoint}: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return Result.Fail(new Error(errorMessage));
            }
            catch (Exception ex)
            {
                var errorMessage = $"Unexpected error fetching Ollama models: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                return Result.Fail(new Error(errorMessage));
            }
        }

        public async Task<Result<bool>> IsModelAvailableAsync(string modelName, CancellationToken cancellationToken = default)
        {
            var modelsResult = await GetAvailableModelsAsync(cancellationToken);
            if (modelsResult.IsFailed)
                return Result.Fail<bool>(modelsResult.Errors);

            var isAvailable = modelsResult.Value.Any(m =>
                string.Equals(m.Name, modelName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(m.DisplayName, modelName, StringComparison.OrdinalIgnoreCase));

            return Result.Ok(isAvailable);
        }

        // Classes pour la désérialisation de la réponse Ollama
        private class OllamaTagsResponse
        {
            public List<OllamaModel>? Models { get; set; }
        }

        private class OllamaModel
        {
            public string? Name { get; set; }
            public long Size { get; set; }
            public DateTime ModifiedAt { get; set; }
            public OllamaModelDetails? Details { get; set; }
        }

        private class OllamaModelDetails
        {
            public string? Family { get; set; }
            public string[]? Format { get; set; }
            public string[]? Families { get; set; }
            public long ParameterSize { get; set; }
            public long QuantizationLevel { get; set; }
        }
    }
}