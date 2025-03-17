using System;
using System.Collections.Generic;

namespace HillMetrics.MIND.API.Contracts.Responses.Flux
{
    /// <summary>
    /// Represents an active flux in the workflow system
    /// </summary>
    public class ActiveFluxDto
    {
        /// <summary>
        /// The ID of the flux
        /// </summary>
        public int FluxId { get; set; }
        
        /// <summary>
        /// The name of the flux
        /// </summary>
        public string FluxName { get; set; }
        
        /// <summary>
        /// The current stage of the workflow
        /// </summary>
        public string Stage { get; set; }
        
        /// <summary>
        /// Detailed description of the current stage
        /// </summary>
        public string Details { get; set; }
        
        /// <summary>
        /// When the workflow started
        /// </summary>
        public DateTime StartTime { get; set; }
        
        /// <summary>
        /// When the workflow was last updated
        /// </summary>
        public DateTime LastUpdateTime { get; set; }
        
        /// <summary>
        /// Duration of the workflow in minutes
        /// </summary>
        public double DurationMinutes { get; set; }
        
        /// <summary>
        /// Percentage of completion (0-100)
        /// </summary>
        public int ProgressPercentage { get; set; }
    }

    /// <summary>
    /// Represents a completed flux in the workflow system
    /// </summary>
    public class CompletedFluxDto
    {
        /// <summary>
        /// The ID of the flux
        /// </summary>
        public int FluxId { get; set; }
        
        /// <summary>
        /// The name of the flux
        /// </summary>
        public string FluxName { get; set; }
        
        /// <summary>
        /// The status of the workflow (Success/Failed)
        /// </summary>
        public string Status { get; set; }
        
        /// <summary>
        /// The final stage of the workflow
        /// </summary>
        public string Stage { get; set; }
        
        /// <summary>
        /// Detailed description of the final stage
        /// </summary>
        public string Details { get; set; }
        
        /// <summary>
        /// When the workflow started
        /// </summary>
        public DateTime StartTime { get; set; }
        
        /// <summary>
        /// When the workflow completed
        /// </summary>
        public DateTime? CompletedAt { get; set; }
        
        /// <summary>
        /// Duration of the workflow in minutes
        /// </summary>
        public double DurationMinutes { get; set; }
    }

    /// <summary>
    /// Represents a history entry in a workflow
    /// </summary>
    public class HistoryEntryDto
    {
        /// <summary>
        /// The stage of this history entry
        /// </summary>
        public string Stage { get; set; }
        
        /// <summary>
        /// Detailed description of this stage
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// When this stage occurred
        /// </summary>
        public DateTime Timestamp { get; set; }
        
        /// <summary>
        /// Time in minutes since the workflow started
        /// </summary>
        public double TimeSinceStart { get; set; }
    }

    /// <summary>
    /// Detailed information about a flux workflow
    /// </summary>
    public class FluxWorkflowDetailsDto
    {
        /// <summary>
        /// The ID of the flux
        /// </summary>
        public int FluxId { get; set; }
        
        /// <summary>
        /// The name of the flux
        /// </summary>
        public string FluxName { get; set; }
        
        /// <summary>
        /// The current stage of the workflow
        /// </summary>
        public string CurrentStage { get; set; }
        
        /// <summary>
        /// Detailed description of the current stage
        /// </summary>
        public string StageDetails { get; set; }
        
        /// <summary>
        /// When the workflow started
        /// </summary>
        public DateTime StartTime { get; set; }
        
        /// <summary>
        /// When the workflow was last updated
        /// </summary>
        public DateTime LastUpdateTime { get; set; }
        
        /// <summary>
        /// When the workflow ended (if completed)
        /// </summary>
        public DateTime? EndTime { get; set; }
        
        /// <summary>
        /// Duration of the workflow in minutes
        /// </summary>
        public double DurationMinutes { get; set; }
        
        /// <summary>
        /// Percentage of completion (0-100)
        /// </summary>
        public int ProgressPercentage { get; set; }
        
        /// <summary>
        /// Whether the workflow is completed
        /// </summary>
        public bool IsCompleted { get; set; }
        
        /// <summary>
        /// Whether the workflow completed successfully
        /// </summary>
        public bool IsSuccessful { get; set; }
        
        /// <summary>
        /// History of all stages in this workflow
        /// </summary>
        public List<HistoryEntryDto> History { get; set; } = new List<HistoryEntryDto>();
    }

    /// <summary>
    /// Summary information about all workflows
    /// </summary>
    public class WorkflowSummaryDto
    {
        /// <summary>
        /// Number of currently active fluxes
        /// </summary>
        public int ActiveFluxCount { get; set; }
        
        /// <summary>
        /// Number of recently completed fluxes
        /// </summary>
        public int RecentlyCompletedCount { get; set; }
        
        /// <summary>
        /// Number of successfully completed fluxes
        /// </summary>
        public int SuccessfulCompletions { get; set; }
        
        /// <summary>
        /// Number of failed fluxes
        /// </summary>
        public int FailedCompletions { get; set; }
        
        /// <summary>
        /// Average completion time in minutes
        /// </summary>
        public double AverageCompletionTimeMinutes { get; set; }
        
        /// <summary>
        /// Count of fluxes by stage
        /// </summary>
        public List<StageCountDto> ByStage { get; set; } = new List<StageCountDto>();
    }

    /// <summary>
    /// Count of fluxes in a particular stage
    /// </summary>
    public class StageCountDto
    {
        /// <summary>
        /// The stage name
        /// </summary>
        public string Stage { get; set; }
        
        /// <summary>
        /// Number of fluxes in this stage
        /// </summary>
        public int Count { get; set; }
    }
} 