using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HillMetrics.Core.API.Responses;
using HillMetrics.Core.Monitoring.Workflow;
using HillMetrics.Core.Workflow;
using HillMetrics.Core.Workflow.Models;
using HillMetrics.MIND.API.Contracts.Responses;
using HillMetrics.MIND.API.Contracts.Responses.Flux;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HillMetrics.MIND.API.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    [ApiController]
    public class WorkflowController : ControllerBase
    {
        private readonly IWorkflowService _workflowService;
        private readonly ILogger<WorkflowController> _logger;

        public WorkflowController(
            IWorkflowService workflowService,
            ILogger<WorkflowController> logger)
        {
            _workflowService = workflowService;
            _logger = logger;
        }

        /// <summary>
        /// Gets the current state of all active workflow fluxes
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<ApiResponseBase<List<ActiveFluxDto>>>> GetActiveFluxes(CancellationToken cancellationToken)
        {
            try
            {
                var activeFluxes = await _workflowService.GetActiveFluxesAsync(cancellationToken);

                var result = activeFluxes.Select(flux => new ActiveFluxDto
                {
                    FluxId = flux.FluxId,
                    FluxName = flux.FluxName,
                    Details = flux.Steps.LastOrDefault()?.Description,
                    StartTime = flux.StartTime,
                    LastUpdateTime = flux.LastUpdateTime,
                    DurationMinutes = flux.Duration.TotalMinutes,
                }).ToList();

                return new ApiResponseBase<List<ActiveFluxDto>>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active fluxes (details : {Message})", ex.Message);
                return new ErrorApiActionResult(new ErrorApiResponse(
                    new Core.API.Exceptions.ApiException($"Error retrieving active fluxes"),
                    System.Net.HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Gets the state of recently completed fluxes
        /// </summary>
        [HttpGet("completed")]
        public async Task<ActionResult<ApiResponseBase<List<CompletedFluxDto>>>> GetCompletedFluxes([FromQuery] int count = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                var completedFluxes = await _workflowService.GetRecentlyCompletedFluxesAsync(count, cancellationToken);

                var result = completedFluxes.Select(flux => new CompletedFluxDto
                {
                    FluxId = flux.FluxId,
                    FluxName = flux.FluxName,
                    Status = flux.EndTime != null ? "Completed" : "Pending",
                    Details = flux.Steps.LastOrDefault()?.Description,
                    StartTime = flux.StartTime,
                    CompletedAt = flux.EndTime,
                    DurationMinutes = flux.Duration.TotalMinutes
                }).ToList();

                return new ApiResponseBase<List<CompletedFluxDto>>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving completed fluxes (details : {Message})", ex.Message);
                return new ErrorApiActionResult(new ErrorApiResponse(
                    new Core.API.Exceptions.ApiException($"Error retrieving completed fluxes"),
                    System.Net.HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Gets the details of a specific flux workflow
        /// </summary>
        [HttpGet("{fluxId}")]
        public async Task<ActionResult<ApiResponseBase<FluxWorkflowDetailsDto>>> GetFluxWorkflowDetails(int fluxId, CancellationToken cancellationToken)
        {
            try
            {
                var fluxState = await _workflowService.GetFluxStateAsync(fluxId, cancellationToken);

                if (fluxState == null)
                {
                    return new ErrorApiActionResult(new ErrorApiResponse(
                        new Core.API.Exceptions.ApiException($"Flux {fluxId} not found"),
                        System.Net.HttpStatusCode.NotFound));
                }

                var result = new FluxWorkflowDetailsDto
                {
                    FluxId = fluxState.FluxId,
                    FluxName = fluxState.FluxName,
                    StageDetails = fluxState.Steps.LastOrDefault()?.Description,
                    StartTime = fluxState.StartTime,
                    LastUpdateTime = fluxState.LastUpdateTime,
                    EndTime = fluxState.EndTime,
                    DurationMinutes = fluxState.Duration.TotalMinutes,
                    IsCompleted = fluxState.IsCompleted,
                    WorkflowId = fluxState.WorkflowId,
                    History = MapWorkflowStepsToHistoryEntries(fluxState.Steps, fluxState.StartTime)
                };

                return new ApiResponseBase<FluxWorkflowDetailsDto>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving details for flux {FluxId} (details : {Message})", fluxId, ex.Message);
                return new ErrorApiActionResult(new ErrorApiResponse(
                    new Core.API.Exceptions.ApiException($"Error retrieving details for flux {fluxId}"),
                    System.Net.HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Gets a global summary of flux workflows
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<ApiResponseBase<WorkflowSummaryDto>>> GetFluxWorkflowSummary(CancellationToken cancellationToken)
        {
            try
            {
                var activeFluxes = await _workflowService.GetActiveFluxesAsync(cancellationToken);
                var completedFluxes = await _workflowService.GetRecentlyCompletedFluxesAsync(100, cancellationToken);

                var summary = new WorkflowSummaryDto
                {
                    ActiveFluxCount = activeFluxes.Count,
                    RecentlyCompletedCount = completedFluxes.Count,
                    AverageCompletionTimeMinutes = completedFluxes.Any()
                        ? completedFluxes.Average(f => f.Duration.TotalMinutes)
                        : 0,
                    ByStage = activeFluxes
                        .GroupBy(f => f.Steps)
                        .Select(g => new StageCountDto
                        {
                            Stage = g.Key.ToString(),
                            Count = g.Count()
                        })
                        .OrderBy(x => x.Stage)
                        .ToList()
                };

                return new ApiResponseBase<WorkflowSummaryDto>(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving flux workflow summary (details : {Message})", ex.Message);
                return new ErrorApiActionResult(new ErrorApiResponse(
                    new Core.API.Exceptions.ApiException($"Error retrieving flux workflow summary"),
                    System.Net.HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Manually triggers cleanup of historical workflow data
        /// </summary>
        [HttpPost("cleanup")]
        public async Task<ActionResult<ApiResponseBase<string>>> TriggerWorkflowCleanup([FromQuery] int daysToKeep = 14, CancellationToken cancellationToken = default)
        {
            try
            {
                await _workflowService.CleanupHistoryAsync(daysToKeep, cancellationToken);
                return new ApiResponseBase<string>($"Cleanup of data older than {daysToKeep} days completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cleanup of historical workflow data (details : {Message})", ex.Message);
                return new ErrorApiActionResult(new ErrorApiResponse(
                    new Core.API.Exceptions.ApiException($"Error during cleanup of historical workflow data"),
                    System.Net.HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Gets a workflow by its unique workflow ID
        /// </summary>
        [HttpGet("by-id/{workflowId}")]
        public async Task<ActionResult<ApiResponseBase<FluxWorkflowDetailsDto>>> GetWorkflowById(Guid workflowId, CancellationToken cancellationToken)
        {
            try
            {
                var workflowState = await _workflowService.GetWorkflowByIdAsync(workflowId, cancellationToken);

                if (workflowState == null)
                {
                    return new ErrorApiActionResult(new ErrorApiResponse(
                        new Core.API.Exceptions.ApiException($"Workflow with ID {workflowId} not found"),
                        System.Net.HttpStatusCode.NotFound));
                }

                var result = new FluxWorkflowDetailsDto
                {
                    FluxId = workflowState.FluxId,
                    FluxName = workflowState.FluxName,
                    StageDetails = workflowState.Steps.LastOrDefault()?.Description,
                    StartTime = workflowState.StartTime,
                    LastUpdateTime = workflowState.LastUpdateTime,
                    EndTime = workflowState.EndTime,
                    DurationMinutes = workflowState.Duration.TotalMinutes,
                    IsCompleted = workflowState.IsCompleted,
                    WorkflowId = workflowState.WorkflowId,
                    History = MapWorkflowStepsToHistoryEntries(workflowState.Steps, workflowState.StartTime)
                };

                return new ApiResponseBase<FluxWorkflowDetailsDto>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow with ID {WorkflowId} (details : {Message})", workflowId, ex.Message);
                return new ErrorApiActionResult(new ErrorApiResponse(
                    new Core.API.Exceptions.ApiException($"Error retrieving workflow with ID {workflowId}"),
                    System.Net.HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Maps workflow steps to a hierarchical structure of history entries
        /// </summary>
        private List<HistoryEntryDto> MapWorkflowStepsToHistoryEntries(List<WorkflowStepModel> steps, DateTime workflowStartTime)
        {
            var result = new List<HistoryEntryDto>();

            foreach (var step in steps)
            {
                var historyEntry = new HistoryEntryDto
                {
                    Id = step.Id,
                    ParentId = step.ParentId,
                    Stage = step.Stage,
                    Description = step.Description,
                    Timestamp = step.Timestamp,
                    TimeSinceStart = (step.Timestamp - workflowStartTime).TotalMinutes,
                    RowsAdded = step.LinesAdded,
                    RowsModified = step.LinesModified,
                    RowsIgnored = step.LinesIgnored,
                    RowsWithErrors = step.LinesWithErrors,
                    IsCompleted = step.IsCompleted,
                    CompletionTimestamp = step.CompletionTimestamp,
                    DurationSeconds = step.DurationSeconds,
                    ActionType = step.ActionType,
                    ActionId = step.ActionId,
                    Metadata = step.Metadata ?? string.Empty,
                    Children = MapWorkflowStepsToHistoryEntries(step.Children, workflowStartTime)
                };

                result.Add(historyEntry);
            }

            return result;
        }
    }
}