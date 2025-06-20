using AutoMapper;
using FluentResults;
using HillMetrics.Core.API.Responses;
using HillMetrics.Core.Common.Email;
using HillMetrics.Core.Messaging.Notification.Market;
using HillMetrics.Core.Time.Trigger;
using HillMetrics.MIND.API.Contracts.Requests.Flux;
using HillMetrics.MIND.API.Contracts.Responses;
using HillMetrics.MIND.API.Contracts.Responses.Flux;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux.Cqrs.Collect;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux.Cqrs.Create;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux.Cqrs.DataPointIdentification;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux.Cqrs.Get;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux.Cqrs.Process;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HillMetrics.Core.Messaging.Bus;
using HillMetrics.Core.Workflow;
using System.Text.Json;
using System.Reflection;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux.Cqrs;
using HillMetrics.Core.Workflow.Models;
using HillMetrics.Core.Contracts;
using HillMetrics.Core.Mediator;
using HillMetrics.Normalized.Domain.Contracts.Market.Executor;
using HillMetrics.Core.Rules.Abstract;
using HillMetrics.MIND.API.Contracts.Responses.FluxDataPoints;
using HillMetrics.MIND.API.Contracts.Requests.FluxDataPoints;
using HillMetrics.Normalized.Domain.Contracts.FluxDataPoints;
using HillMetrics.Normalized.Domain.Contracts.Market.Cqrs;
using HillMetrics.Core;

namespace HillMetrics.MIND.API.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    //[EnableRateLimiting("allow5000requestsPerSecond_fixed")]
    public class FluxController(
        IHMediator mediator,
        IMapper mapper,
        IWorkflowService workflowService,
        ILogger<FluxController> logger,
        IServiceScopeFactory serviceScopeFactory,
        IApiRequestContext apiRequestContext) : BaseHillMetricsController(mediator)
    {
        #region Flux
        /// <summary>
        /// Search for fluxes following the given criteria
        /// </summary>
        /// <param name="request">Criteria search</param>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<ActionResult<CustomMindPagedApiResponseBase<FluxSearchResponse>>> SearchAsync([FromQuery] FluxSearchRequest request)
        {
            var result = await Mediator.Send(mapper.Map<SearchFluxQuery>(request));

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new CustomMindPagedApiResponseBase<FluxSearchResponse>(mapper.Map<List<FluxSearchResponse>>(result.Value.Results), result.Value.NbTotalRows);
        }

        /// <summary>
        /// Get the details of a flux
        /// </summary>
        /// <param name="id">The flux identifier</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<FluxResponseWrapper>> GetFluxAsync(int id)
        {
            var result = await Mediator.Send(new FluxQuery() { FluxId = id });

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            // Create a concrete wrapper for the response
            var wrapper = new FluxResponseWrapper(mapper.Map<FluxResponse>(result.Value.Flux));

            return wrapper;
        }

        /// <summary>
        /// Create a new flux
        /// </summary>
        /// <param name="fluxRequest">The flux informations</param>
        /// <returns>true if success, false otherwise</returns>
        [HttpPost, AllowAnonymous]
        public async Task<ActionResult<bool>> CreateFlux(FluxRequest fluxRequest)
        {
            var fluxCommand = CreateFluxCommand.Create();
            fluxCommand = builder(mapper, fluxRequest, fluxCommand);

            var result = await Mediator.Send(fluxCommand);

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return true;
        }

        /// <summary>
        /// Update a flux
        /// </summary>
        /// <param name="fluxId">The flux identifier</param>
        /// <param name="fluxRequest">The flux informations</param>
        /// <returns>true if success, false otherwise</returns>
        [HttpPut, AllowAnonymous]
        public async Task<ActionResult<bool>> UpdateFlux(int fluxId, FluxRequest fluxRequest)
        {
            var fluxCommand = CreateFluxCommand.Create().WithId(fluxId);
            fluxCommand = builder(mapper, fluxRequest, fluxCommand);

            var result = await Mediator.Send(fluxCommand);

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return true;
        }

        /// <summary>
        /// Delete a flux
        /// </summary>
        /// <param name="fluxId">The flux identifier</param>
        /// <returns>true if success, false otherwise</returns>
        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteFlux(int fluxId)
        {
            var result = await Mediator.Send(new DeleteFluxCommand(fluxId));

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return true;
        }

        private static CreateFluxCommand builder(IMapper mapper, FluxRequest fluxRequest, CreateFluxCommand fluxCommand)
        {
            if (fluxRequest.FluxName is not null)
                fluxCommand = fluxCommand.WithName(fluxRequest.FluxName);

            if (fluxRequest.FluxDescription is not null)
                fluxCommand = fluxCommand.WithDescription(fluxRequest.FluxDescription);

            if (fluxRequest.FluxComment is not null)
                fluxCommand = fluxCommand.WithComment(fluxRequest.FluxComment);

            if (fluxRequest.SourceId is not null)
                fluxCommand = fluxCommand.WithSourceProvider(fluxRequest.SourceId.Value);

            if (fluxRequest.FluxFinancialType is not null)
                fluxCommand = fluxCommand.WithFluxFinancialType(fluxRequest.FluxFinancialType.Value);

            if (fluxRequest.ProcessTriggerPeriod is not null)
                fluxCommand = fluxCommand.WithProcessTriggerPeriod(mapper.Map<TriggerPeriodDto, TriggerPeriod>(fluxRequest.ProcessTriggerPeriod));

            if (fluxRequest.FetchTriggerPeriod is not null)
                fluxCommand = fluxCommand.WithFetchTriggerPeriod(mapper.Map<TriggerPeriodDto, TriggerPeriod>(fluxRequest.FetchTriggerPeriod));

            if (fluxRequest.FluxMetadata is not null)
                fluxCommand = fluxCommand.WithFluxMetadata(mapper.Map<FluxMetadataDto, FluxMetadata>(fluxRequest.FluxMetadata));

            if (fluxRequest.FluxType is not null)
                fluxCommand = fluxCommand.WithFluxType(fluxRequest.FluxType.Value);

            if (fluxRequest.FluxState is not null)
                fluxCommand = fluxCommand.WithFluxState(fluxRequest.FluxState.Value);

            if (fluxRequest.CanHaveConcurrencyMultiFetching is not null)
                fluxCommand = fluxCommand.WithConcurrencyMultiFetching(fluxRequest.CanHaveConcurrencyMultiFetching.Value);

            return fluxCommand;
        }

        /// <summary>
        /// Force the processing of normalized financial prices, triggering the same flow as when prices are automatically inserted
        /// </summary>
        /// <param name="request">Request parameters including FinancialIds and Dates (FluxId and Currency are optional)</param>
        /// <returns>Operation status</returns>
        [HttpPost("force-financial-price-processing")]
        public async Task<ActionResult<ApiResponseBase<ForceFinancialPriceProcessingResponse>>> ForceFinancialPriceProcessingAsync(ForceFinancialPriceProcessingRequest request)
        {
            try
            {
                // Basic validation
                if (request.FinancialIds == null || !request.FinancialIds.Any())
                    return BadRequest("At least one FinancialId must be provided");

                if (request.FinancialIds.Any(id => id <= 0))
                    return BadRequest("All FinancialIds must be greater than 0");

                // Map request to command
                var command = new ForceFinancialPriceProcessingCommand
                {
                    FinancialIds = request.FinancialIds,
                    FluxId = request.FluxId,
                    Currency = request.Currency,
                    Dates = request.Dates
                };

                // Send the command to the handler
                var result = await Mediator.Send(command);

                if (result.IsFailed)
                    return new ErrorApiActionResult(result.Errors.ToApiResult());

                // Map the result to response
                return new ApiResponseBase<ForceFinancialPriceProcessingResponse>(
                    new ForceFinancialPriceProcessingResponse
                    {
                        Success = result.Value.Success,
                        FinancialIdCount = result.Value.FinancialIdCount,
                        DatesCount = result.Value.DatesCount
                    }
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while forcing financial price processing");
                return new ErrorApiActionResult(
                    new ErrorApiResponse(new Core.API.Exceptions.ApiException($"An error occurred while forcing price processing: {ex.Message}"), System.Net.HttpStatusCode.BadRequest)
                );
            }
        }

        /// <summary>
        /// Force the fetch of a flux asynchronously (non-blocking)
        /// </summary>
        /// <param name="id">The flux identifier</param>
        /// <returns>Status indication that the operation has started</returns>
        [HttpGet("{id}/force-fetch-async")]
        public async Task<ActionResult<ApiResponseBase<ProcessStartedResponse>>> ForceFetchBackgroundAsync(int id)
        {
            try
            {
                var requestAudit = apiRequestContext.GetAudit();

                // Get flux name by querying the flux
                var fluxResult = await Mediator.Send(new FluxQuery() { FluxId = id });
                if (fluxResult.IsFailed)
                {
                    return new ErrorApiActionResult(fluxResult.Errors.ToApiResult());
                }

                var fluxName = fluxResult.Value.Flux.FluxName;

                var r = await workflowService.StartWorkflowTrackingAsync(id, WorkflowStage.Created, "Background fetching started", FluxActionType.Initializing, id);
                var existingWorkflowId = r.Value.Item1;
                int stepId = r.Value.Item2;

                // Store in context for subsequent background operations
                WorkflowContext.SetCurrentWorkflowId(id, existingWorkflowId);

                // Start the background processing task without awaiting it
                _ = Task.Run(async () =>
                {
                    // Create a new scope for the long-running operation
                    using var scope = serviceScopeFactory.CreateScope();

                    try
                    {
                        // Resolve required services from the new scope
                        var scopedMediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var scopedLogger = scope.ServiceProvider.GetRequiredService<ILogger<FluxController>>();

                        scopedLogger.LogInformation("Starting asynchronous fetch for flux {FluxId} with workflow {WorkflowId}",
                            id, existingWorkflowId);

                        // Execute the operation with services from the new scope
                        var command = FetchFluxCommand.CreateFromBackOffice(id, stepId);
                        command.Audit = requestAudit;

                        var fetchResult = await scopedMediator.Send(command);

                        if (fetchResult.IsSuccess)
                        {
                            scopedLogger.LogInformation("Async fetch completed successfully for flux {FluxId}", id);
                        }
                        else
                        {
                            scopedLogger.LogWarning("Async fetch failed for flux {FluxId}: {Errors}",
                                id, string.Join(", ", fetchResult.Errors.Select(e => e.Message)));
                        }
                    }
                    catch (Exception ex)
                    {
                        // Use logger from the scope
                        var scopedLogger = scope.ServiceProvider.GetRequiredService<ILogger<FluxController>>();
                        scopedLogger.LogError(ex, "Error in asynchronous flux fetch for flux {FluxId}", id);
                    }
                });

                // Create a response with the workflow ID for tracking
                var response = new ProcessStartedResponse
                {
                    Message = $"Flux fetch operation started for flux {id}. The operation will continue in the background.",
                    FluxId = id,
                    WorkflowId = existingWorkflowId
                };

                return new ApiResponseBase<ProcessStartedResponse>(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error initiating asynchronous flux fetch for flux {FluxId}", id);
                return new ErrorApiActionResult(
                    new ErrorApiResponse(
                        new Core.API.Exceptions.ApiException($"Error starting asynchronous flux fetch: {ex.Message}"),
                        System.Net.HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Force the process of a flux asynchronously (non-blocking)
        /// </summary>
        /// <param name="id">The flux identifier</param>
        /// <returns>Status indication that the operation has started</returns>
        [HttpGet("{id}/force-process-async")]
        public async Task<ActionResult<ApiResponseBase<ProcessStartedResponse>>> ForceProcessBackgroundAsync(int id)
        {
            try
            {
                var requestAudit = apiRequestContext.GetAudit();

                // Start the background processing task without awaiting it
                _ = Task.Run(async () =>
                {
                    // Create a new scope using the factory instead of the provider
                    using var scope = serviceScopeFactory.CreateScope();

                    try
                    {
                        // Resolve required services from the new scope
                        var scopedMediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var scopedLogger = scope.ServiceProvider.GetRequiredService<ILogger<FluxController>>();

                        scopedLogger.LogInformation("Starting asynchronous process for flux {FluxId}", id);

                        var command = new ProcessFluxCommand()
                        {
                            FluxId = id,
                            CalledManually = true
                        };
                        command.Audit = requestAudit;

                        // Execute the operation with services from the new scope
                        var processResult = await scopedMediator.Send(command);

                        if (processResult.IsSuccess)
                        {
                            scopedLogger.LogInformation("Async process completed successfully for flux {FluxId}", id);
                        }
                        else
                        {
                            scopedLogger.LogWarning("Async process failed for flux {FluxId}: {Errors}",
                                id, string.Join(", ", processResult.Errors.Select(e => e.Message)));
                        }
                    }
                    catch (Exception ex)
                    {
                        var scopedLogger = scope.ServiceProvider.GetRequiredService<ILogger<FluxController>>();
                        scopedLogger.LogError(ex, "Error in asynchronous flux process for flux {FluxId}", id);
                    }
                });

                // Create a response with the workflow ID for tracking
                var response = new ProcessStartedResponse
                {
                    Message = $"Flux process operation started for flux {id}. The operation will continue in the background.",
                    FluxId = id,
                    WorkflowId = new Guid()
                };

                return new ApiResponseBase<ProcessStartedResponse>(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error initiating asynchronous flux process for flux {FluxId}", id);
                return new ErrorApiActionResult(
                    new ErrorApiResponse(
                        new Core.API.Exceptions.ApiException($"Error starting asynchronous flux process: {ex.Message}"),
                        System.Net.HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Forces the processing of a specific flux fetching content history in the background
        /// </summary>
        /// <param name="fluxId">The ID of the flux</param>
        /// <param name="fluxFetchingHistoryId">The ID of the flux fetching history to process</param>
        /// <returns>A response indicating that the processing has started</returns>
        [HttpGet("fetching-history/{fluxFetchingHistoryId}/force-process-async")]
        public async Task<ActionResult<ApiResponseBase<ProcessStartedResponse>>> ForceProcessElementFetchBackgroundAsync(int fluxId, int fluxFetchingHistoryId)
        {
            try
            {
                var requestAudit = apiRequestContext.GetAudit();

                var existingWorkflow = await workflowService.GetWorkflowFromFetchingAsync(fluxFetchingHistoryId);
                var fetchStep = await workflowService.TryFindFetchingContentStepAsync(fluxFetchingHistoryId);

                // Start the background processing task without awaiting it
                _ = Task.Run(async () =>
                {
                    // Create a new scope using the factory instead of the provider
                    using var scope = serviceScopeFactory.CreateScope();

                    try
                    {
                        // Resolve required services from the new scope
                        var scopedMediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var scopedLogger = scope.ServiceProvider.GetRequiredService<ILogger<FluxController>>();

                        var command = new ProcessElementCommand()
                        {
                            FluxFetchingHistoryId = fluxFetchingHistoryId,
                            CalledManually = true,
                            FluxId = fluxId,
                            //WorkflowStepId = fetchStep.Value
                        };
                        command.Audit = requestAudit;

                        // Execute the operation with services from the new scope
                        var result = await scopedMediator.Send(command);

                        if (result.IsSuccess)
                        {
                            scopedLogger.LogInformation("Async process completed successfully for flux fetching content {ContentId}", fluxFetchingHistoryId);
                        }
                        else
                        {
                            scopedLogger.LogWarning("Async process failed for flux fetching content {ContentId}: {Errors}",
                                fluxFetchingHistoryId, string.Join(", ", result.Errors.Select(e => e.Message)));
                        }
                    }
                    catch (Exception ex)
                    {
                        var scopedLogger = scope.ServiceProvider.GetRequiredService<ILogger<FluxController>>();
                        scopedLogger.LogError(ex, "Error in asynchronous flux fetching content process for content {ContentId}", fluxFetchingHistoryId);
                    }
                });

                // Create a response with the workflow ID for tracking
                var response = new ProcessStartedResponse
                {
                    Message = $"Flux fetching content process operation started for content {fluxFetchingHistoryId}. The operation will continue in the background.",
                    FluxId = fluxId,
                    WorkflowId = existingWorkflow.Value.WorkflowId
                };

                return new ApiResponseBase<ProcessStartedResponse>(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error initiating asynchronous flux fetching content process for content {ContentId}", fluxFetchingHistoryId);
                return new ErrorApiActionResult(
                    new ErrorApiResponse(
                        new Core.API.Exceptions.ApiException($"Error starting asynchronous flux fetching content process: {ex.Message}"),
                        System.Net.HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Upload and process a file for a manual flux
        /// </summary>
        /// <param name="fluxId">The flux identifier</param>
        /// <param name="fileName">The name of the uploaded file</param>
        /// <param name="file">The uploaded file</param>
        /// <returns>Status indication that the operation has started</returns>
        [HttpPost("{fluxId}/upload-manual")]
        public async Task<ActionResult<ApiResponseBase<ProcessStartedResponse>>> FetchManualFluxAsync(
            int fluxId,
            [FromForm] string fileName,
            IFormFile file)
        {
            try
            {
                var requestAudit = apiRequestContext.GetAudit();

                // Validate inputs
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file provided or file is empty");
                }

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = file.FileName;
                }

                // Get flux details to verify it's manual
                var fluxResult = await Mediator.Send(new FluxQuery() { FluxId = fluxId });
                if (fluxResult.IsFailed)
                {
                    return new ErrorApiActionResult(fluxResult.Errors.ToApiResult());
                }

                var flux = fluxResult.Value.Flux;
                if (flux.FluxType != FluxType.Manual)
                {
                    return BadRequest("This flux is not a manual flux");
                }

                // Convert IFormFile to Stream
                using var fileStream = file.OpenReadStream();
                using var memoryStream = new MemoryStream();
                await fileStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // Create workflow tracking
                var workflowResult = await workflowService.StartWorkflowTrackingAsync(fluxId, WorkflowStage.Created, "Manual file upload started", FluxActionType.Initializing, fluxId);
                var existingWorkflowId = workflowResult.Value.Item1;
                int stepId = workflowResult.Value.Item2;

                // Store in context for subsequent background operations
                WorkflowContext.SetCurrentWorkflowId(fluxId, existingWorkflowId);

                logger.LogInformation("Starting manual flux processing for flux {FluxId} with workflow {WorkflowId}",
                    fluxId, existingWorkflowId);

                // Create command with manual content
                var command = FetchFluxCommand.CreateManual(fluxId, stepId, memoryStream, fileName);
                command.Audit = requestAudit;

                var fetchResult = await Mediator.Send(command);

                if (fetchResult.IsSuccess)
                {
                    logger.LogInformation("Manual flux processing completed successfully for flux {FluxId}", fluxId);

                    // Create a response with the workflow ID for tracking
                    var response = new ProcessStartedResponse
                    {
                        Message = $"Manual flux upload operation completed successfully for flux {fluxId}.",
                        FluxId = fluxId,
                        WorkflowId = existingWorkflowId
                    };

                    return new ApiResponseBase<ProcessStartedResponse>(response);
                }
                else
                {
                    logger.LogWarning("Manual flux processing failed for flux {FluxId}: {Errors}",
                        fluxId, string.Join(", ", fetchResult.Errors.Select(e => e.Message)));

                    return new ErrorApiActionResult(fetchResult.Errors.ToApiResult());
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing manual flux upload for flux {FluxId}", fluxId);
                return new ErrorApiActionResult(
                    new ErrorApiResponse(
                        new Core.API.Exceptions.ApiException($"Error processing manual flux upload: {ex.Message}"),
                        System.Net.HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Fetch emails and process metadata
        /// </summary>
        /// <param name="request">Request containing email fetching parameters</param>
        /// <returns>Result of the email fetching operation</returns>
        [HttpPost("fetch-emails")]
        public async Task<ActionResult<ApiResponseBase<FetchEmailMetadataCommandResult>>> FetchEmailsAsync()
        {
            var result = await Mediator.Send(FetchEmailMetadataCommand.CreateDefault());
            
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());
            
            return new ApiResponseBase<FetchEmailMetadataCommandResult>(result.Value);
        }
        #endregion

        #region FinancialDataPoint

        /// <summary>
        /// Identify automatically the list of data points of the flux. By default it choose the last fetching history
        /// It calls the detectors (with AI) to identify the data points
        /// </summary>
        /// <param name="id">The flux identifier</param>
        /// <returns></returns>
        [HttpGet("{id}/financial-data-point/identify")]
        public async Task<ActionResult<FluxIdentificationResponse>> IdentifyDataPointAsync(int id)
        {
            var result = await Mediator.Send(new IdentifyFinancialDataPointCommand() { FluxId = id });

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new FluxIdentificationResponse()
            {
                FluxIdentificationHistoryId = result.Value.FluxIdentificationHistory.Id,
                DetectionResult = result.Value.DetectionResult.Select(x => new IdentificationItemResponse()
                {
                    DetectorName = x.DetectorName,
                    DataPoint = x.DataPoint,
                    FinancialDataPointId = x.FinancialDataPointId,
                    IsSuccess = true,
                    MetadataMapping = x.MetadataMapping,
                    Score = x.Score
                }).ToList()
            };
        }

        /// <summary>
        /// Identify automatically the list of data points of a flux fetching history id.
        /// It calls the detectors (with AI) to identify the data points
        /// </summary>
        /// <param name="fluxFetchingId">The flux fetching history identifier</param>
        /// <returns></returns>
        [HttpGet("financial-data-point/identify/{fluxFetchingId}")]
        public async Task<ActionResult<FluxIdentificationResponse>> IdentifyDataPointFromSpecificHistoryAsync(int fluxFetchingId)
        {
            var result = await Mediator.Send(new IdentifyFinancialDataPointCommand() { FluxFetchingId = fluxFetchingId });

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new FluxIdentificationResponse()
            {
                FluxIdentificationHistoryId = result.Value.FluxIdentificationHistory.Id,
                DetectionResult = result.Value.DetectionResult.Select(x => new IdentificationItemResponse()
                {
                    DetectorName = x.DetectorName,
                    DataPoint = x.DataPoint,
                    FinancialDataPointId = x.FinancialDataPointId,
                    IsSuccess = true,
                    MetadataMapping = x.MetadataMapping,
                    Score = x.Score
                }).ToList()
            };
        }

        /// <summary>
        /// Validate (or invalidate) a financial data point
        /// If it is invalidate, the request.HumanMetadataMapping property is mandatory
        /// </summary>
        /// <param name="request"></param>
        /// <returns>true if success, false otherwise</returns>
        [HttpPost("financial-data-point/validate")]
        public async Task<ActionResult<bool>> ValidateFinancialDataPointAsync(FluxValidateFinancialDataPointRequest request)
        {
            var result = await Mediator.Send(new ManageDataPointIdentificationCommand(request.FinancialDataPointId, request.IsValidated, request.HumanMetadataMapping));

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return true;
        }

        ///// <summary>
        ///// Try to identify the list of financial data points from a file upload
        ///// </summary>
        ///// <param name="file">The file to check the financial data point</param>
        ///// <param name="contentType">The content type of the file (for security)</param>
        ///// <returns>The list of datapoints that has been checked</returns>
        //[HttpPost("financial-data-point/check")]
        //public async Task<ActionResult<FluxIdentifyDataPointResponse>> IdentifyFinancialDataPointFromFileUploadAsync([FromForm] IFormFile file, ContentType contentType)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("No file uploaded or the file is empty.");

        //    // Define the path to save the file (ensure this path exists or create it)
        //    var savePath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles", file.FileName);

        //    // Save the file
        //    Stream stream;
        //    using (stream = new FileStream(savePath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }

        //    var result = await Mediator.Send(new CheckFinancialDataPointQuery(file.FileName, stream, contentType));

        //    if (result.IsFailed)
        //        return new ErrorAPIResult(result.Errors.ToApiException());

        //    return new FluxIdentifyDataPointResponse()
        //    {
        //        DetectionResult = result.Value.DetectionResult.Select(x => new FluxIdentifyDataPointItemResponse()
        //        {
        //            DetectorName = x.DetectorName,
        //            DataPoint = x.DataPoint,
        //            IsSuccess = true,
        //            MetadataMapping = x.MetadataMapping,
        //            Score = x.Score
        //        }).ToList()
        //    };
        //}
        #endregion

        #region Fetching
        /// <summary>
        /// Search for flux fetching history following the given criteria
        /// </summary>
        /// <param name="request">The search criterias</param>
        /// <returns>The flux fetching history that matched the requests filters</returns>
        [HttpGet("fetching-history/search")]
        public async Task<ActionResult<CustomMindPagedApiResponseBase<FluxFetchingSearchResponse>>> SearchFetchingHistoryAsync([FromQuery] FluxFetchingSearchRequest request)
        {
            var result = await Mediator.Send(mapper.Map<SearchFluxFetchingQuery>(request));

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new CustomMindPagedApiResponseBase<FluxFetchingSearchResponse>(mapper.Map<List<FluxFetchingSearchResponse>>(result.Value.Results), result.Value.NbTotalRows);
        }

        /// <summary>
        /// Get the details of a specific fetching history
        /// </summary>
        /// <param name="fetchingHistoryId">The fetching history identifier</param>
        /// <returns></returns>
        [HttpGet("fetching-history/{fetchingHistoryId}")]
        public async Task<ActionResult<ApiResponseBase<FluxFetchingResponse>>> GetFetchingHistoryAsync(int fetchingHistoryId)
        {
            var result = await Mediator.Send(new FluxFetchHistoryQuery(fetchingHistoryId));

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new ApiResponseBase<FluxFetchingResponse>(mapper.Map<FluxFetchingResponse>(result.Value));
        }

        /// <summary>
        /// Get the details of a specific fetching content
        /// </summary>
        /// <param name="fetchingContentId">The fetching content identifier</param>
        /// <returns></returns>
        [HttpGet("fetching-history/content/{fetchingContentId}")]
        public async Task<ActionResult<ApiResponseBase<FluxFetchingContentHistoryResponse>>> GetFetchingContentAsync(int fetchingContentId)
        {
            var result = await Mediator.Send(new FluxFetchContentHistoryQuery(fetchingContentId));

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new ApiResponseBase<FluxFetchingContentHistoryResponse>(
                mapper.Map<FluxFetchingContentHistoryResponse>(result.Value.FluxFetchingContent));
        }

        /// <summary>
        /// Delete a specific fetching history if it doesn't have any content with Success status
        /// </summary>
        /// <param name="fetchingHistoryId">The fetching history identifier to delete</param>
        /// <returns>True if deleted successfully, error details if it failed</returns>
        [HttpDelete("fetching-history/{fetchingHistoryId}")]
        public async Task<ActionResult<ApiResponseBase<bool>>> DeleteFetchingHistoryAsync(int fetchingHistoryId)
        {
            try
            {
                var result = await Mediator.Send(new DeleteFluxHistoryCommand(fetchingHistoryId));

                if (result.IsFailed)
                    return new ErrorApiActionResult(result.Errors.ToApiResult());

                return new ApiResponseBase<bool>(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting fetching history: {FetchingHistoryId}", fetchingHistoryId);
                return new ErrorApiActionResult(new ErrorApiResponse(
                    new Core.API.Exceptions.ApiException($"Error deleting fetching history: {ex.Message}"),
                    System.Net.HttpStatusCode.InternalServerError));
            }
        }

        #region Simulate Process Element
        /// <summary>
        /// Simulate the processing of a specific flux fetching history
        /// </summary>
        /// <param name="fetchingHistoryId">The ID of the fetching history to simulate processing</param>
        /// <returns>Simulation result with detailed processing information</returns>
        [HttpGet("fetching-history/{fetchingHistoryId}/simulate-process")]
        public async Task<ActionResult<ApiResponseBase<SimulateProcessElementResponse>>> SimulateProcessElementAsync(int fetchingHistoryId)
        {
            try
            {
                var command = new SimulateProcessElementCommand
                {
                    FluxFetchingHistoryId = fetchingHistoryId
                };

                var result = await Mediator.Send(command);

                if (result.IsFailed)
                    return new ErrorApiActionResult(result.Errors.ToApiResult());

                var extractedData = result.Value.ExtractedData != null ? MapToGlobalExecutorResponseDto(result.Value.ExtractedData) : null;
                var commandExecution = result.Value.CommandExecution != null ? MapToCommandExecutionResponseDto(result.Value.CommandExecution) : null;

                var response = new SimulateProcessElementResponse
                {
                    Status = (SimulateProcessElementResponse.SimulationStatus)result.Value.Status,
                    FluxId = result.Value.FluxId,
                    ProcessingTimeMs = result.Value.ProcessingTimeMs,
                    ExtractedData = extractedData,
                    CommandExecution = commandExecution,
                    SimulationSummary = BuildSimulationSummary(extractedData, commandExecution)
                };

                return new ApiResponseBase<SimulateProcessElementResponse>(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error simulating process for fetching history: {FetchingHistoryId}", fetchingHistoryId);
                return new ErrorApiActionResult(new ErrorApiResponse(
                    new Core.API.Exceptions.ApiException($"Error simulating process: {ex.Message}"),
                    System.Net.HttpStatusCode.InternalServerError));
            }
        }

        private static GlobalExecutorResponseDto MapToGlobalExecutorResponseDto(GlobalExecutorResult source)
        {
            return new GlobalExecutorResponseDto
            {
                Json = source.Json,
                ExecutorResults = source.ExecutorResults.Select(MapToExecutorResponseDto).ToList()
            };
        }

        private static ExecutorResponseDto MapToExecutorResponseDto(ExecutorResult source)
        {
            return new ExecutorResponseDto
            {
                Rows = source.Rows.Select(MapToExecutorRowResponseDto).ToList()
            };
        }

        private static ExecutorRowResponseDto MapToExecutorRowResponseDto(ExecutorRowResult source)
        {
            return new ExecutorRowResponseDto
            {
                FinancialTechnicalDataPoint = source.FinancialTechnicalDataPoint,
                RuleResult = MapToRuleResultDto(source.RuleResult)
            };
        }

        private static RuleResultDto MapToRuleResultDto(IRuleResult source)
        {
            return new RuleResultDto
            {
                RuleName = source.RuleName,
                IsSuccess = source.IsSuccess,
                ErrorMessage = source.ErrorMessage,
                ValidationMessages = source.ValidationMessages.Select(vm => new ValidationMessageDto
                {
                    Message = vm.Message,
                    Level = vm.Severity.ToString(),
                    Property = vm.Code
                }).ToList(),
                OutputValues = source.OutputValues,
                OriginalData = source.GetOriginalData(),
                ProcessedData = source.GetData()
            };
        }

        private static CommandExecutionResponseDto MapToCommandExecutionResponseDto(CommandExecutionResult source)
        {
            return new CommandExecutionResponseDto
            {
                ExecutableCommandsByFinancialId = source.ExecutableCommandsByFinancialId.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Select(cmd => new CommandWithElementsDto
                    {
                        CommandName = ExtractCommandName(cmd),
                        Description = ExtractCommandDescription(cmd),
                        IsList = IsCollectionCommand(cmd),
                        Elements = ExtractCommandElements(cmd)
                    }).ToList()
                )
            };
        }

        /// <summary>
        /// Extracts the command name from a command object
        /// </summary>
        private static string ExtractCommandName(IMarketCommand command)
        {
            if (command == null)
                return string.Empty;

            // If command implements IBusinessDescription, use BusinessName
            if (command is IBusinessDescription businessDescription && !string.IsNullOrWhiteSpace(businessDescription.BusinessName))
            {
                return businessDescription.BusinessName;
            }

            // Fallback to type name
            var fullTypeName = command.GetType().Name;
            return fullTypeName;
        }

        /// <summary>
        /// Extracts the command description from a command object
        /// </summary>
        private static string ExtractCommandDescription(IMarketCommand command)
        {
            if (command == null)
                return string.Empty;

            // If command implements IBusinessDescription, use BusinessDescription
            if (command is IBusinessDescription businessDescription && !string.IsNullOrWhiteSpace(businessDescription.BusinessDescription))
            {
                return businessDescription.BusinessDescription;
            }

            return string.Empty;
        }

        /// <summary>
        /// Determines if a command is a collection command (inherits from AbstractCollectionCommand)
        /// </summary>
        private static bool IsCollectionCommand(IMarketCommand command)
        {
            if (command == null)
                return false;

            // Check if the command type has AbstractCollectionCommand in its inheritance hierarchy
            var commandType = command.GetType();

            // Check if the type or any of its base types is a generic type with AbstractCollectionCommand as the generic type definition
            while (commandType != null && commandType != typeof(object))
            {
                if (commandType.IsGenericType)
                {
                    var genericTypeDef = commandType.GetGenericTypeDefinition();
                    if (genericTypeDef.Name.StartsWith("AbstractCollectionCommand"))
                    {
                        return true;
                    }
                }

                // Check if the base type name contains AbstractCollectionCommand
                if (commandType.Name.Contains("AbstractCollectionCommand") ||
                    (commandType.BaseType != null && commandType.BaseType.Name.Contains("AbstractCollectionCommand")))
                {
                    return true;
                }

                commandType = commandType.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Extracts the command elements/parameters from a command object
        /// </summary>
        private static List<string> ExtractCommandElements(IMarketCommand command)
        {
            if (command == null)
                return new List<string>();

            // Check if it's an AbstractCollectionCommand by looking for Items property
            var itemsProperty = command.GetType().GetProperty("Items", BindingFlags.Public | BindingFlags.Instance);
            if (itemsProperty != null)
            {
                try
                {
                    var itemsValue = itemsProperty.GetValue(command);
                    if (itemsValue is System.Collections.ICollection collection)
                    {
                        return new List<string> { $"[items.Count: {collection.Count}]" };
                    }
                }
                catch
                {
                    // If we can't read the Items property, fall back to empty list
                    return new List<string>();
                }
            }

            // For AbstractCommand or other types, return empty list
            return new List<string>();
        }

        /// <summary>
        /// Builds a comprehensive simulation summary from extracted data and command execution results
        /// </summary>
        private static SimulationSummaryDto BuildSimulationSummary(GlobalExecutorResponseDto? extractedData, CommandExecutionResponseDto? commandExecution)
        {
            var summary = new SimulationSummaryDto();

            // Calculate validation summary
            if (extractedData != null)
            {
                summary.ValidationSummary = CalculateValidationSummary(extractedData);
                summary.HasValidationErrors = summary.ValidationSummary.ValidationErrors.Any();
            }

            // Calculate command statistics
            if (commandExecution != null)
            {
                summary.CommandsToExecute = commandExecution.ExecutableCommandsByFinancialId.Values
                    .SelectMany(commands => commands)
                    .Count();

                summary.FinancialIdsAffected = commandExecution.ExecutableCommandsByFinancialId.Keys.Count;
            }

            // Determine if processing can proceed
            summary.CanProceedWithProcessing = summary.ValidationSummary.SuccessRate >= 80.0 && // At least 80% success rate
                                             summary.CommandsToExecute > 0 && // Has commands to execute
                                             !summary.HasValidationErrors; // No critical validation errors

            return summary;
        }

        /// <summary>
        /// Calculates validation statistics from extracted data
        /// </summary>
        private static ValidationSummaryDto CalculateValidationSummary(GlobalExecutorResponseDto extractedData)
        {
            var validationSummary = new ValidationSummaryDto();
            var allRuleResults = new List<RuleResultDto>();

            // Collect all rule results from all executors
            foreach (var executor in extractedData.ExecutorResults)
            {
                foreach (var row in executor.Rows)
                {
                    if (row.RuleResult != null)
                    {
                        allRuleResults.Add(row.RuleResult);
                    }
                }
            }

            validationSummary.TotalRulesExecuted = allRuleResults.Count;
            validationSummary.SuccessfulValidations = allRuleResults.Count(r => r.IsSuccess);
            validationSummary.FailedValidations = allRuleResults.Count(r => !r.IsSuccess);

            // Calculate success rate
            if (validationSummary.TotalRulesExecuted > 0)
            {
                validationSummary.SuccessRate = (double)validationSummary.SuccessfulValidations / validationSummary.TotalRulesExecuted * 100;
            }

            // Collect validation errors
            validationSummary.ValidationErrors = allRuleResults
                .Where(r => !r.IsSuccess)
                .Select(r => new ValidationErrorDto
                {
                    RuleName = r.RuleName,
                    ErrorMessage = r.ErrorMessage,
                    DataPoint = ExtractDataPointFromRuleResult(r),
                    OriginalValue = r.OriginalData?.ToString() ?? string.Empty
                })
                .ToList();

            return validationSummary;
        }

        /// <summary>
        /// Extracts a data point identifier from rule result for error reporting
        /// </summary>
        private static string ExtractDataPointFromRuleResult(RuleResultDto ruleResult)
        {
            // Try to extract meaningful data point information
            if (ruleResult.OutputValues?.ContainsKey("DataPoint") == true)
            {
                return ruleResult.OutputValues["DataPoint"]?.ToString() ?? "Unknown";
            }

            if (ruleResult.OutputValues?.ContainsKey("Property") == true)
            {
                return ruleResult.OutputValues["Property"]?.ToString() ?? "Unknown";
            }

            // Look for validation messages with property information
            var propertyMessage = ruleResult.ValidationMessages?.FirstOrDefault(vm => !string.IsNullOrEmpty(vm.Property));
            if (propertyMessage != null)
            {
                return propertyMessage.Property;
            }

            // Use rule name as fallback if available
            if (!string.IsNullOrEmpty(ruleResult.RuleName))
            {
                return ruleResult.RuleName;
            }

            return "Unknown DataPoint";
        }
        #endregion
        #endregion

        #region Processing
        /// <summary>
        /// Search for flux processing history following the given criteria
        /// </summary>
        /// <param name="request">The search criterias</param>
        /// <returns>The flux processing history that matched the requests filters</returns>
        [HttpGet("processing-history/search")]
        public async Task<ActionResult<CustomMindPagedApiResponseBase<FluxProcessingSearchReponse>>> SearchProcessingHistoryAsync([FromQuery] FluxProcessingSearchRequest request)
        {
            var result = await Mediator.Send(mapper.Map<SearchFluxProcessingFluxQuery>(request));

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new CustomMindPagedApiResponseBase<FluxProcessingSearchReponse>(mapper.Map<List<FluxProcessingSearchReponse>>(result.Value.Results), result.Value.NbTotalRows);
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="file"></param>
        ///// <param name="nbToLinesToAnalyse"></param>
        ///// <returns></returns>
        ///// <exception cref="NotImplementedException"></exception>
        //[HttpPost("process-data/simulation")]
        //public async Task<ActionResult<bool>> SimulateProcessFromDataAsync([FromForm] IFormFile file, int nbToLinesToAnalyse)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Get the details of a specific processing history
        /// </summary>
        /// <param name="processingHistoryId">The processing history identifier</param>
        /// <returns></returns>
        [HttpGet("processing-history/{processingHistoryId}")]
        public async Task<ActionResult<ApiResponseBase<FluxProcessingResponse>>> GetProcessingHistoryAsync(int processingHistoryId)
        {
            var result = await Mediator.Send(new FluxProcessingQuery(processingHistoryId));

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            var response = mapper.Map<FluxProcessingResponse>(result.Value);

            // Add statistics
            response.Statistics = new ProcessingStatisticsResponse
            {
                TotalErrors = result.Value.RowsError,
                TotalRowsInserted = result.Value.RowsAdded,
                TotalRowsUpdated = result.Value.RowsUpdated,
                TotalRowsIgnored = result.Value.RowsIgnored
            };

            return new ApiResponseBase<FluxProcessingResponse>(response);
        }
        #endregion

        #region Flux errors
        /// <summary>
        /// Search for flux errors history following the given criteria
        /// </summary>
        /// <param name="request">The search criterias</param>
        /// <returns>The flux errors history that matched the requests filters</returns>
        [HttpGet("errors/search")]
        public async Task<ActionResult<CustomMindPagedApiResponseBase<FluxErrorSearchResponse>>> SearchFluxErrorsAsync([FromQuery] FluxErrorSearchRequest request)
        {
            var result = await Mediator.Send(mapper.Map<SearchFluxErrorQuery>(request));

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new CustomMindPagedApiResponseBase<FluxErrorSearchResponse>(mapper.Map<List<FluxErrorSearchResponse>>(result.Value.Results), result.Value.NbTotalRows);
        }

        /// <summary>
        /// Get the details of a specific error
        /// </summary>
        /// <param name="errorId">The error identifier</param>
        /// <returns></returns>
        [HttpGet("errors/{errorId}")]
        public async Task<ActionResult<ApiResponseBase<FluxErrorResponse>>> GetErrorAsync(int errorId)
        {
            var result = await Mediator.Send(new FluxErrorQuery(errorId));

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new ApiResponseBase<FluxErrorResponse>(mapper.Map<FluxErrorResponse>(result.Value));
        }

        /// <summary>
        /// Delete multiple flux errors
        /// </summary>
        /// <param name="errorIds">List of error IDs to delete</param>
        /// <returns>True if all errors were deleted successfully</returns>
        [HttpDelete("errors")]
        public async Task<ActionResult<ApiResponseBase<bool>>> DeleteFluxErrorsAsync([FromBody] List<int> errorIds)
        {
            try
            {
                var result = await Mediator.Send(new DeleteFluxErrorCommand(errorIds));

                if (result.IsFailed)
                    return new ErrorApiActionResult(result.Errors.ToApiResult());

                return new ApiResponseBase<bool>(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting flux errors: {ErrorIds}", string.Join(", ", errorIds));
                return new ErrorApiActionResult(new ErrorApiResponse(
                    new Core.API.Exceptions.ApiException($"Error deleting flux errors: {ex.Message}"),
                    System.Net.HttpStatusCode.InternalServerError));
            }
        }
        #endregion

        #region Rule Errors
        /// <summary>
        /// Search for rule errors following the given criteria
        /// </summary>
        /// <param name="request">The search criterias</param>
        /// <returns>The rule errors that matched the requests filters</returns>
        [HttpGet("rule-errors/search")]
        public async Task<ActionResult<CustomMindPagedApiResponseBase<RuleErrorSearchResponse>>> SearchRuleErrorsAsync([FromQuery] RuleErrorSearchRequest request)
        {
            var result = await Mediator.Send(mapper.Map<SearchRuleErrorQuery>(request));

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new CustomMindPagedApiResponseBase<RuleErrorSearchResponse>(mapper.Map<List<RuleErrorSearchResponse>>(result.Value.Results), result.Value.NbTotalRows);
        }
        #endregion

        #region FluxDataPointsRealtions

        [HttpPost("financial-data-point/fluxes/link")]
        public async Task<ActionResult<ListDataPointFluxesResponse>> LinkFluxesToDataPointAsync(LinkFluxesToDataPointRequest request)
        {
            var command = new LinkFluxesToDataPointCommand(new LinkFluxesToDataPointModel(request.FinancialDataPointId, request.FluxIds));
            var result = await Mediator.Send(command);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());


            List<Contracts.Responses.FluxDataPoints.FluxDataPointDto> dtos = result.Value.Data.Select(FluxDataPointDtoMapper.FromDomain).ToList();

            return new ListDataPointFluxesResponse(dtos, result.Value.TotalRecords);
        }

        [HttpGet("financial-data-point/{financialDataPointId}/fluxes/search")]
        public async Task<ActionResult<ListDataPointFluxesResponse>> SearchDataPointFluxesAsync([FromRoute] int financialDataPointId)
        {
            var query = new SearchDataPointFluxesQuery(financialDataPointId);
            var result = await Mediator.Send(query);
            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            List<Contracts.Responses.FluxDataPoints.FluxDataPointDto> dtos = result.Value.Data.Select(FluxDataPointDtoMapper.FromDomain).ToList();

            return new ListDataPointFluxesResponse(dtos, result.Value.TotalRecords);
        }

        #endregion
    }
}