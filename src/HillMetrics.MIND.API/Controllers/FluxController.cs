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
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux.Cqrs;

namespace HillMetrics.MIND.API.Controllers
{
    [Route("api/v{v:apiVersion}/[controller]")]
    //[EnableRateLimiting("allow5000requestsPerSecond_fixed")]
    public class FluxController(IMediator mediator, IMapper mapper, IWorkflowTracker workflowTracker, IWorkflowService workflowService, ILogger<FluxController> logger, IServiceScopeFactory serviceScopeFactory) : BaseHillMetricsController(mediator)
    {
        #region Flux
        /// <summary>
        /// Search for fluxes following the given criteria
        /// </summary>
        /// <param name="request">Criteria search</param>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<ActionResult<PagedApiResponseBase<FluxSearchResponse>>> SearchAsync([FromQuery] FluxSearchRequest request)
        {
            var result = await Mediator.Send(mapper.Map<SearchFluxQuery>(request));

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new PagedApiResponseBase<FluxSearchResponse>(mapper.Map<List<FluxSearchResponse>>(result.Value.Results), result.Value.NbTotalRows);
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

        ///// <summary>
        ///// Force the fetch of a flux
        ///// </summary>
        ///// <param name="id">The flux identifier</param>
        ///// <returns></returns>
        //[HttpGet("{id}/force-fetch")]
        //public async Task<ActionResult<FluxForceFetchResponse>> ForceFetch(int id)
        //{
        //    var command = FetchFluxCommand.Create(id, Task.FromResult(new List<Mail>()));
        //    command.CalledManually = true;
        //    var result = await mediator.Send(command);

        //    if (result.IsFailed)
        //        return new ErrorApiActionResult(result.Errors.ToApiResult());

        //    return mapper.Map<FluxForceFetchResponse>(result.Value);
        //}

        ///// <summary>
        ///// Force the process of a flux
        ///// </summary>
        ///// <param name="id">The flux identifier</param>
        ///// <returns></returns>
        //[HttpGet("{id}/force-process")]
        //public async Task<ActionResult<FluxForceProcessResponse>> ForceProcess(int id)
        //{
        //    var result = await mediator.Send(new ProcessFluxCommand() {
        //        FluxId = id,
        //        CalledManually = true
        //    });

        //    if (result.IsFailed)
        //        return new ErrorApiActionResult(result.Errors.ToApiResult());

        //    return mapper.Map<FluxForceProcessResponse>(result.Value);
        //}

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
                // Use the workflow service to start/get a workflow for this flux
                var result = await workflowService.StartWorkflowTrackingAsync(
                    id,
                    WorkflowStage.Created,
                    "Background fetching started",
                    FluxActionType.Initializing,
                    id);

                if (result.IsFailed)
                {
                    return new ErrorApiActionResult(new ErrorApiResponse(
                        new Core.API.Exceptions.ApiException(result.Errors.First().Message),
                        System.Net.HttpStatusCode.InternalServerError));
                }

                var workflowId = result.Value.Item1;

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
                            id, workflowId);

                        // Execute the operation with services from the new scope
                        var command = FetchFluxCommand.Create(id, Task.FromResult(new List<Mail>()));

                        command.CalledManually = true;
                        command.WorkflowStepId = result.Value.Item2;

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
                    WorkflowId = workflowId
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

                        // Execute the operation with services from the new scope
                        var processResult = await scopedMediator.Send(new ProcessFluxCommand() {
                            FluxId = id,
                            CalledManually = true
                        });

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
                    WorkflowId = workflowId
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
        public async Task<ActionResult<PagedApiResponseBase<FluxFetchingSearchResponse>>> SearchFetchingHistoryAsync([FromQuery] FluxFetchingSearchRequest request)
        {
            var result = await Mediator.Send(mapper.Map<SearchFluxFetchingQuery>(request));

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new PagedApiResponseBase<FluxFetchingSearchResponse>(mapper.Map<List<FluxFetchingSearchResponse>>(result.Value.Results), result.Value.NbTotalRows);
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
        #endregion

        #region Processing
        /// <summary>
        /// Search for flux processing history following the given criteria
        /// </summary>
        /// <param name="request">The search criterias</param>
        /// <returns>The flux processing history that matched the requests filters</returns>
        [HttpGet("processing-history/search")]
        public async Task<ActionResult<PagedApiResponseBase<FluxProcessingSearchReponse>>> SearchProcessingHistoryAsync([FromQuery] FluxProcessingSearchRequest request)
        {
            var result = await Mediator.Send(mapper.Map<SearchFluxProcessingFluxQuery>(request));

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new PagedApiResponseBase<FluxProcessingSearchReponse>(mapper.Map<List<FluxProcessingSearchReponse>>(result.Value.Results), result.Value.NbTotalRows);
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
                TotalErrors = result.Value.TotalErrors,
                TotalRowsInserted = result.Value.TotalRowsInserted,
                TotalRowsUpdated = result.Value.TotalRowsUpdated,
                TotalRowsIgnored = result.Value.TotalRowsIgnored
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
        public async Task<ActionResult<PagedApiResponseBase<FluxErrorSearchResponse>>> SearchFluxErrorsAsync([FromQuery] FluxErrorSearchRequest request)
        {
            var result = await Mediator.Send(mapper.Map<SearchFluxErrorQuery>(request));

            if (result.IsFailed)
                return new ErrorApiActionResult(result.Errors.ToApiResult());

            return new PagedApiResponseBase<FluxErrorSearchResponse>(mapper.Map<List<FluxErrorSearchResponse>>(result.Value.Results), result.Value.NbTotalRows);
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

        #region Workflow

        /// <summary>
        /// Gets the current state of all active workflow fluxes
        /// </summary>
        [HttpGet("workflow/active")]
        public async Task<ActionResult<ApiResponseBase<List<ActiveFluxDto>>>> GetActiveFluxes(CancellationToken cancellationToken)
        {
            try
            {
                var activeFluxes = await workflowTracker.GetActiveFluxesAsync(cancellationToken);

                var result = activeFluxes.Select(flux => new ActiveFluxDto
                {
                    FluxId = flux.FluxId,
                    FluxName = flux.FluxName,
                    Stage = flux.CurrentStage.ToString(),
                    Details = flux.Steps.LastOrDefault()?.Description,
                    StartTime = flux.StartTime,
                    LastUpdateTime = flux.LastUpdateTime,
                    DurationMinutes = flux.Duration.TotalMinutes,
                    ProgressPercentage = flux.ProgressPercentage
                }).ToList();

                return new ApiResponseBase<List<ActiveFluxDto>>(result);
            }
            catch (Exception ex)
            {
                return new ErrorApiActionResult(new ErrorApiResponse(new Core.API.Exceptions.ApiException($"Error retrieving active fluxes (details : {ex.Message})"), System.Net.HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Gets the state of recently completed fluxes
        /// </summary>
        [HttpGet("workflow/completed")]
        public async Task<ActionResult<ApiResponseBase<List<CompletedFluxDto>>>> GetCompletedFluxes([FromQuery] int count = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                var completedFluxes = await workflowTracker.GetRecentlyCompletedFluxesAsync(count, cancellationToken);

                var result = completedFluxes.Select(flux => new CompletedFluxDto
                {
                    FluxId = flux.FluxId,
                    FluxName = flux.FluxName,
                    Status = flux.IsSuccessful ? "Success" : "Failed",
                    Stage = flux.CurrentStage.ToString(),
                    Details = flux.Steps.LastOrDefault()?.Description,
                    StartTime = flux.StartTime,
                    CompletedAt = flux.EndTime,
                    DurationMinutes = flux.Duration.TotalMinutes
                }).ToList();

                return new ApiResponseBase<List<CompletedFluxDto>>(result);
            }
            catch (Exception ex)
            {
                return new ErrorApiActionResult(new ErrorApiResponse(new Core.API.Exceptions.ApiException($"Error retrieving completed fluxes (details : {ex.Message})"), System.Net.HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Gets the details of a specific flux workflow
        /// </summary>
        [HttpGet("workflow/{fluxId}")]
        public async Task<ActionResult<ApiResponseBase<FluxWorkflowDetailsDto>>> GetFluxWorkflowDetails(int fluxId, CancellationToken cancellationToken)
        {
            try
            {
                var fluxState = await workflowTracker.GetFluxStateAsync(fluxId, cancellationToken);

                if (fluxState == null)
                {
                    return new ErrorApiActionResult(new ErrorApiResponse(new Core.API.Exceptions.ApiException($"Flux {fluxId} not found"), System.Net.HttpStatusCode.NotFound));
                }

                var result = new FluxWorkflowDetailsDto
                {
                    FluxId = fluxState.FluxId,
                    FluxName = fluxState.FluxName,
                    CurrentStage = fluxState.CurrentStage.ToString(),
                    StageDetails = fluxState.Steps.LastOrDefault()?.Description,
                    StartTime = fluxState.StartTime,
                    LastUpdateTime = fluxState.LastUpdateTime,
                    EndTime = fluxState.EndTime,
                    DurationMinutes = fluxState.Duration.TotalMinutes,
                    ProgressPercentage = fluxState.ProgressPercentage,
                    IsCompleted = fluxState.IsCompleted,
                    IsSuccessful = fluxState.IsSuccessful,
                    History = fluxState.Steps.Select(h => new HistoryEntryDto
                    {
                        Stage = h.Stage.ToString(),
                        Description = h.Description,
                        Timestamp = h.Timestamp,
                        TimeSinceStart = (h.Timestamp - fluxState.StartTime).TotalMinutes
                    }).OrderByDescending(h => h.Timestamp).ToList()
                };

                return new ApiResponseBase<FluxWorkflowDetailsDto>(result);
            }
            catch (Exception ex)
            {
                return new ErrorApiActionResult(new ErrorApiResponse(new Core.API.Exceptions.ApiException($"Error retrieving details for flux {fluxId} (details : {ex.Message})"), System.Net.HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Gets a global summary of flux workflows
        /// </summary>
        [HttpGet("workflow/summary")]
        public async Task<ActionResult<ApiResponseBase<WorkflowSummaryDto>>> GetFluxWorkflowSummary(CancellationToken cancellationToken)
        {
            try
            {
                var activeFluxes = await workflowTracker.GetActiveFluxesAsync(cancellationToken);
                var completedFluxes = await workflowTracker.GetRecentlyCompletedFluxesAsync(100, cancellationToken);

                var summary = new WorkflowSummaryDto
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
                return new ErrorApiActionResult(new ErrorApiResponse(new Core.API.Exceptions.ApiException($"Error retrieving flux workflow summary (details : {ex.Message})"), System.Net.HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Manually triggers cleanup of historical workflow data
        /// </summary>
        [HttpPost("workflow/cleanup")]
        public async Task<ActionResult<ApiResponseBase<string>>> TriggerWorkflowCleanup([FromQuery] int daysToKeep = 14, CancellationToken cancellationToken = default)
        {
            try
            {
                await workflowTracker.CleanupHistoryAsync(daysToKeep, cancellationToken);
                return new ApiResponseBase<string>($"Cleanup of data older than {daysToKeep} days completed successfully");
            }
            catch (Exception ex)
            {
                return new ErrorApiActionResult(new ErrorApiResponse(new Core.API.Exceptions.ApiException($"Error during cleanup of historical workflow data (details : {ex.Message})"), System.Net.HttpStatusCode.InternalServerError));
            }
        }

        /// <summary>
        /// Gets a workflow by its unique workflow ID
        /// </summary>
        [HttpGet("workflow/by-id/{workflowId}")]
        public async Task<ActionResult<ApiResponseBase<FluxWorkflowDetailsDto>>> GetWorkflowById(Guid workflowId, CancellationToken cancellationToken)
        {
            try
            {
                var workflowState = await workflowTracker.GetWorkflowByIdAsync(workflowId, cancellationToken);

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
                    CurrentStage = workflowState.CurrentStage.ToString(),
                    StageDetails = workflowState.Steps.LastOrDefault()?.Description,
                    StartTime = workflowState.StartTime,
                    LastUpdateTime = workflowState.LastUpdateTime,
                    EndTime = workflowState.EndTime,
                    DurationMinutes = workflowState.Duration.TotalMinutes,
                    ProgressPercentage = workflowState.ProgressPercentage,
                    IsCompleted = workflowState.IsCompleted,
                    IsSuccessful = workflowState.IsSuccessful,
                    History = workflowState.Steps.Select(h => new HistoryEntryDto
                    {
                        Stage = h.Stage.ToString(),
                        Description = h.Description,
                        Timestamp = h.Timestamp,
                        TimeSinceStart = (h.Timestamp - workflowState.StartTime).TotalMinutes,
                        RowsAdded = h.LinesAdded,
                        RowsModified = h.LinesModified,
                        RowsIgnored = h.LinesIgnored,
                        RowsWithErrors = h.LinesWithErrors
                    }).OrderByDescending(h => h.Timestamp).ToList(),
                    WorkflowId = workflowState.WorkflowId
                };

                return new ApiResponseBase<FluxWorkflowDetailsDto>(result);
            }
            catch (Exception ex)
            {
                return new ErrorApiActionResult(new ErrorApiResponse(
                    new Core.API.Exceptions.ApiException($"Error retrieving workflow with ID {workflowId} (details : {ex.Message})"),
                    System.Net.HttpStatusCode.InternalServerError));
            }
        }

        #endregion
    }
}