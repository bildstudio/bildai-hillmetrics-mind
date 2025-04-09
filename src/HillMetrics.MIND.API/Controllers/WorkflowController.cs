using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HillMetrics.Core.API.Responses;
using HillMetrics.Core.Monitoring.Workflow;
using HillMetrics.Core.Workflow;
using HillMetrics.MIND.API.Contracts.Responses.Flux;
using HillMetrics.Normalized.Domain.Workflow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HillMetrics.MIND.API.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    [ApiController]
    public class WorkflowController : ControllerBase
    {
        private readonly IWorkflowTracker _workflowTracker;
        private readonly ILogger<WorkflowController> _logger;

        public WorkflowController(
            IWorkflowTracker workflowTracker,
            ILogger<WorkflowController> logger)
        {
            _workflowTracker = workflowTracker;
            _logger = logger;
        }

        /// <summary>
        /// Gets the current state of all active workflow fluxes
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveFluxes(CancellationToken cancellationToken)
        {
            try
            {
                var activeFluxes = await _workflowTracker.GetActiveFluxesAsync(cancellationToken);

                var result = activeFluxes.Select(flux => new
                {
                    flux.FluxId,
                    flux.FluxName,
                    Stage = flux.CurrentStage.ToString(),
                    Details = flux.Steps.LastOrDefault()?.Description,
                    flux.StartTime,
                    flux.LastUpdateTime,
                    DurationMinutes = flux.Duration.TotalMinutes,
                    flux.ProgressPercentage
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active fluxes");
                return StatusCode(500, new { Error = "Error retrieving active fluxes" });
            }
        }

        /// <summary>
        /// Gets the state of recently completed fluxes
        /// </summary>
        [HttpGet("completed")]
        public async Task<IActionResult> GetCompletedFluxes([FromQuery] int count = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                var completedFluxes = await _workflowTracker.GetRecentlyCompletedFluxesAsync(count, cancellationToken);

                var result = completedFluxes.Select(flux => new
                {
                    flux.FluxId,
                    flux.FluxName,
                    Status = flux.IsSuccessful ? "Success" : "Failed",
                    Stage = flux.CurrentStage.ToString(),
                    Details = flux.Steps.LastOrDefault()?.Description,
                    flux.StartTime,
                    CompletedAt = flux.EndTime,
                    DurationMinutes = flux.Duration.TotalMinutes
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving completed fluxes");
                return StatusCode(500, new { Error = "Error retrieving completed fluxes" });
            }
        }

        /// <summary>
        /// Gets the details of a specific flux
        /// </summary>
        [HttpGet("{fluxId}")]
        public async Task<IActionResult> GetFluxDetails(int fluxId, CancellationToken cancellationToken)
        {
            try
            {
                var fluxState = await _workflowTracker.GetFluxStateAsync(fluxId, cancellationToken);

                if (fluxState == null)
                {
                    return NotFound(new { Error = $"Flux {fluxId} not found" });
                }

                var result = new
                {
                    fluxState.FluxId,
                    fluxState.FluxName,
                    CurrentStage = fluxState.CurrentStage.ToString(),
                    StageDetails = fluxState.Steps.LastOrDefault()?.Description,
                    fluxState.StartTime,
                    fluxState.LastUpdateTime,
                    fluxState.EndTime,
                    DurationMinutes = fluxState.Duration.TotalMinutes,
                    fluxState.ProgressPercentage,
                    fluxState.IsCompleted,
                    fluxState.IsSuccessful,
                    History = fluxState.Steps.Select(h => new
                    {
                        Stage = h.Stage.ToString(),
                        h.Description,
                        h.Timestamp,
                        TimeSinceStart = (h.Timestamp - fluxState.StartTime).TotalMinutes
                    }).OrderByDescending(h => h.Timestamp).ToList()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving details for flux {FluxId}", fluxId);
                return StatusCode(500, new { Error = $"Error retrieving details for flux {fluxId}" });
            }
        }

        /// <summary>
        /// Gets a global summary of fluxes
        /// </summary>
        [HttpGet("summary")]
        public async Task<IActionResult> GetFluxSummary(CancellationToken cancellationToken)
        {
            try
            {
                var activeFluxes = await _workflowTracker.GetActiveFluxesAsync(cancellationToken);
                var completedFluxes = await _workflowTracker.GetRecentlyCompletedFluxesAsync(100, cancellationToken);

                var summary = new
                {
                    ActiveFluxCount = activeFluxes.Count,
                    RecentlyCompletedCount = completedFluxes.Count,
                    SuccessfulCompletions = completedFluxes.Count(f => f.IsSuccessful),
                    FailedCompletions = completedFluxes.Count(f => !f.IsSuccessful),
                    AverageCompletionTimeMinutes = completedFluxes.Any()
                        ? completedFluxes.Average(f => f.Duration.TotalMinutes)
                        : 0,
                    ByStage = activeFluxes
                        .GroupBy(f => f.CurrentStage)
                        .Select(g => new
                        {
                            Stage = g.Key.ToString(),
                            Count = g.Count()
                        })
                        .OrderBy(x => x.Stage)
                        .ToList()
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving flux summary");
                return StatusCode(500, new { Error = "Error retrieving flux summary" });
            }
        }

        /// <summary>
        /// Manually triggers cleanup of historical data
        /// </summary>
        [HttpPost("cleanup")]
        public async Task<IActionResult> TriggerCleanup([FromQuery] int daysToKeep = 14, CancellationToken cancellationToken = default)
        {
            try
            {
                await _workflowTracker.CleanupHistoryAsync(daysToKeep, cancellationToken);
                return Ok(new { Message = $"Cleanup of data older than {daysToKeep} days completed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cleanup of historical data");
                return StatusCode(500, new { Error = "Error during cleanup of historical data" });
            }
        }
    }
}