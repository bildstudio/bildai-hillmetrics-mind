using HillMetrics.Core.Financial.DataPoint;
using HillMetrics.Core.Rules.Abstract;

namespace HillMetrics.MIND.API.Contracts.Responses.Flux
{
    public class SimulateProcessElementResponse
    {
        public enum SimulationStatus
        {
            Success,
            PartialSuccess,
            Fail,
            NoRawId,
            ContentNull,
            NoCustomProcessingImplementation,
            JsonConversionFailed,
            DataExtractionFailed,
            ValidationErrors,
            NoExecutableCommands
        }

        public SimulationStatus Status { get; set; } = SimulationStatus.Fail;
        public int FluxId { get; set; }
        public long ProcessingTimeMs { get; set; }

        /// <summary>
        /// The comprehensive simulation summary
        /// </summary>
        public SimulationSummaryDto? SimulationSummary { get; set; }

        /// <summary>
        /// The extracted data from the JSON content
        /// </summary>
        public GlobalExecutorResponseDto? ExtractedData { get; set; }

        /// <summary>
        /// The determined executable commands
        /// </summary>
        public CommandExecutionResponseDto? CommandExecution { get; set; }

        /// <summary>
        /// Financial instruments that will be created
        /// </summary>
        public List<FinancialInstrumentToCreateDto>? FinancialsToCreate { get; set; }

        /// <summary>
        /// Complete formatted report for easy viewing
        /// </summary>
        public string? CompleteReport { get; set; }

        /// <summary>
        /// List of issues that prevent processing
        /// </summary>
        public List<string> ProcessingIssues { get; set; } = new();

        /// <summary>
        /// Indicates if the flux is ready for production processing
        /// </summary>
        public bool IsReadyForProcessing { get; set; }
    }

    /// <summary>
    /// Summary of the entire simulation
    /// </summary>
    public class SimulationSummaryDto
    {
        public ValidationSummaryDto ValidationSummary { get; set; } = new();
        public int CommandsToExecute { get; set; }
        public int FinancialIdsAffected { get; set; }
        public bool HasValidationErrors { get; set; }
        public bool CanProceedWithProcessing { get; set; }
    }

    /// <summary>
    /// Summary of validation results
    /// </summary>
    public class ValidationSummaryDto
    {
        public int TotalRulesExecuted { get; set; }
        public int SuccessfulValidations { get; set; }
        public int FailedValidations { get; set; }
        public double SuccessRate { get; set; }
        public List<ValidationErrorDto> ValidationErrors { get; set; } = new();
    }

    /// <summary>
    /// Validation error information
    /// </summary>
    public class ValidationErrorDto
    {
        public string RuleName { get; set; } = string.Empty;
        public string DataPoint { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string OriginalValue { get; set; } = string.Empty;
    }

    /// <summary>
    /// Financial instrument that will be created
    /// </summary>
    public class FinancialInstrumentToCreateDto
    {
        public Dictionary<string, object> DataPoints { get; set; } = new();

        /// <summary>
        /// Gets a human-readable description of the financial instrument
        /// </summary>
        public string GetDescription()
        {
            var items = new List<string>();

            foreach (var kvp in DataPoints)
            {
                items.Add($"{kvp.Key}: {kvp.Value}");
            }

            return string.Join(", ", items);
        }
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
        /// Gets the name of the rule that was executed
        /// </summary>
        public string RuleName { get; set; } = string.Empty;

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
        public Dictionary<int, List<CommandWithElementsDto>> ExecutableCommandsByFinancialId { get; set; } = new();
    }

    /// <summary>
    /// Represents a command with its name and associated elements
    /// </summary>
    public class CommandWithElementsDto
    {
        /// <summary>
        /// The name/type of the command
        /// </summary>
        public string CommandName { get; set; } = string.Empty;

        /// <summary>
        /// The business description of the command
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if this command is a collection command (AbstractCollectionCommand) or not
        /// </summary>
        public bool IsList { get; set; }

        /// <summary>
        /// The list of elements/parameters for this command
        /// </summary>
        public List<string> Elements { get; set; } = new();
    }
}