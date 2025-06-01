using HillMetrics.Core.Financial.DataPoint;
using HillMetrics.Core.Rules.Abstract;

namespace HillMetrics.MIND.API.Contracts.Responses.Flux
{
    public class SimulateProcessElementResponse
    {
        public enum SimulationStatus
        {
            Success,
            Fail,
            NoRawId,
            ContentNull,
            NoCustomProcessingImplementation,
            JsonConversionFailed,
            DataExtractionFailed,
            NoExecutableCommands
        }

        public SimulationStatus Status { get; set; } = SimulationStatus.Fail;
        public int FluxId { get; set; }
        public long ProcessingTimeMs { get; set; }

        /// <summary>
        /// The extracted data from the JSON content
        /// </summary>
        public GlobalExecutorResponseDto? ExtractedData { get; set; }

        /// <summary>
        /// The determined executable commands
        /// </summary>
        public CommandExecutionResponseDto? CommandExecution { get; set; }
    }

    public class GlobalExecutorResponseDto
    {
        public string Json { get; set; } = string.Empty;
        public List<ExecutorResponseDto> ExecutorResults { get; set; } = new();
    }

    public class ExecutorResponseDto
    {
        public List<ExecutorRowResponseDto> Rows { get; set; } = new();
    }

    public class ExecutorRowResponseDto
    {
        public FinancialTechnicalDataPoint FinancialTechnicalDataPoint { get; set; }
        public RuleResultDto RuleResult { get; set; } = new();
    }

    public class RuleResultDto
    {
        /// <summary>
        /// Gets whether the rule was successfully applied
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Gets any error message if the rule failed
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Gets any validation messages or warnings
        /// </summary>
        public List<ValidationMessageDto> ValidationMessages { get; set; } = new();

        /// <summary>
        /// Gets a dictionary of additional output values produced by the rule
        /// </summary>
        public Dictionary<string, object> OutputValues { get; set; } = new();

        /// <summary>
        /// Gets the original data before rule application
        /// </summary>
        public object? OriginalData { get; set; }

        /// <summary>
        /// Gets the processed data after rule application
        /// </summary>
        public object? ProcessedData { get; set; }
    }

    public class ValidationMessageDto
    {
        public string Message { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty; // Info, Warning, Error
        public string Property { get; set; } = string.Empty;
    }

    public class CommandExecutionResponseDto
    {
        public Dictionary<int, List<string>> ExecutableCommandsByFinancialId { get; set; } = new();
        public List<string> PartiallyMatchedCommands { get; set; } = new();
    }
}