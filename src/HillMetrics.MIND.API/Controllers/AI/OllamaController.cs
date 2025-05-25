using HillMetrics.Core.AI.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace HillMetrics.MIND.API.Controllers.AI
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class OllamaController : ControllerBase
    {
        private readonly IOllamaModelService _ollamaModelService;
        private readonly ILogger<OllamaController> _logger;

        public OllamaController(IOllamaModelService ollamaModelService, ILogger<OllamaController> logger)
        {
            _ollamaModelService = ollamaModelService;
            _logger = logger;
        }

        /// <summary>
        /// Récupère la liste des modèles Ollama disponibles localement
        /// </summary>
        /// <returns>Liste des modèles avec leurs informations</returns>
        [HttpGet("models")]
        public async Task<IActionResult> GetAvailableModels(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching available Ollama models");

            var result = await _ollamaModelService.GetAvailableModelsAsync(cancellationToken);

            if (result.IsFailed)
            {
                _logger.LogError("Failed to fetch Ollama models: {Errors}", string.Join(", ", result.Errors.Select(e => e.Message)));
                return BadRequest(new { error = "Unable to fetch available models", details = result.Errors.Select(e => e.Message) });
            }

            var models = result.Value.Select(m => new
            {
                name = m.Name,
                displayName = m.DisplayName,
                tag = m.Tag,
                size = m.Size,
                sizeFormatted = FormatBytes(m.Size),
                modifiedAt = m.ModifiedAt,
                family = m.Family,
                parameterSize = m.ParameterSize,
                parameterSizeFormatted = m.ParameterSize > 0 ? $"{m.ParameterSize / 1_000_000_000.0:F1}B" : "Unknown"
            }).OrderBy(m => m.parameterSize).ThenBy(m => m.displayName);

            return Ok(new
            {
                success = true,
                count = result.Value.Count,
                models = models
            });
        }

        /// <summary>
        /// Vérifie si un modèle spécifique est disponible
        /// </summary>
        /// <param name="modelName">Nom du modèle à vérifier</param>
        /// <returns>Statut de disponibilité du modèle</returns>
        [HttpGet("models/{modelName}/available")]
        public async Task<IActionResult> IsModelAvailable(string modelName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(modelName))
            {
                return BadRequest(new { error = "Model name is required" });
            }

            _logger.LogInformation("Checking availability for model: {ModelName}", modelName);

            var result = await _ollamaModelService.IsModelAvailableAsync(modelName, cancellationToken);

            if (result.IsFailed)
            {
                _logger.LogError("Failed to check model availability: {Errors}", string.Join(", ", result.Errors.Select(e => e.Message)));
                return BadRequest(new { error = "Unable to check model availability", details = result.Errors.Select(e => e.Message) });
            }

            return Ok(new
            {
                modelName = modelName,
                available = result.Value
            });
        }

        private static string FormatBytes(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number /= 1024;
                counter++;
            }
            return $"{number:n1} {suffixes[counter]}";
        }
    }
}