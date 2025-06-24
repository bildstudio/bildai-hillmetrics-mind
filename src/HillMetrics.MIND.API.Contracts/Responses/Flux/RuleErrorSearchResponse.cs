using HillMetrics.Core.Financial.DataPoint;
using HillMetrics.Core.Rules.Abstract;

namespace HillMetrics.MIND.API.Contracts.Responses.Flux
{
    public class RuleErrorSearchResponse
    {
        public required int Id { get; set; }
        public int? FinancialId { get; set; }
        public required int FluxId { get; set; }
        public required string FluxName { get; set; }
        public required int FluxProcessingContentId { get; set; }
        public required int WorkflowStepId { get; set; }
        public required FinancialTechnicalDataPoint DataPoint { get; set; }
        public required string ErrorValue { get; set; }
        public required string ErrorMessage { get; set; }
        public required RuleType RuleType { get; set; }
        public required bool IsProcessed { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public required DateTime CreationDate { get; set; }
        public string? AdditionalData { get; set; }
        public required RuleSeverity RuleSeverity { get; set; }
        public required string RuleName { get; set; }
    }
}