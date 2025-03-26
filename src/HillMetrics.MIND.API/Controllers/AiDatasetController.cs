using AutoMapper;
using HillMetrics.Core.API.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset.Cqrs.ElementValue;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset.Cqrs.FileDataMapping;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset.Cqrs.FileUpload;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset.Cqrs.FinancialDataPoint;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;
using HillMetrics.MIND.API.Contracts.Responses;
using HillMetrics.MIND.API.Contracts.Requests.AiDataset;

namespace HillMetrics.MIND.API.Controllers;

public class AiDatasetController(IMediator mediator, IMapper mapper, ILogger<AiDatasetController> logger) : BaseHillMetricsController(mediator)
{
    #region FileUpload

    /// <summary>
    /// Upload a new file for AI dataset processing
    /// </summary>
    /// <param name="request">File upload request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Details of the uploaded file</returns>
    [HttpPost("file-upload")]
    public async Task<ActionResult<ApiResponseBase<FileUpload>>> CreateFileUpload(
        [FromForm] FileUploadRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling file upload request for file: {FileName}", request.File.FileName);

        using var stream = request.File.OpenReadStream();
        var command = new CreateFileUploadCommand
        {
            FileName = request.File.FileName,
            ContentType = request.File.ContentType,
            FileStream = stream,
            Difficulty = request.Difficulty
        };

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<FileUpload>(result.Value);
    }

    /// <summary>
    /// Get all uploaded files
    /// </summary>
    /// <param name="request">Get all file uploads request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all file uploads</returns>
    [HttpGet("file-uploads")]
    public async Task<ActionResult<ApiResponseBase<List<FileUpload>>>> GetAllFileUploads(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllFileUploadsQuery(), cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<List<FileUpload>>(result.Value);
    }

    /// <summary>
    /// Get details of a specific file upload
    /// </summary>
    /// <param name="fileUploadId">ID of the file upload</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>File upload details</returns>
    [HttpGet("file-upload/{fileUploadId}")]
    public async Task<ActionResult<ApiResponseBase<FileUpload>>> GetFileUpload(int fileUploadId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting file upload with ID: {FileUploadId}", fileUploadId);
        var query = new GetFileUploadQuery { FileUploadId = fileUploadId };
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<FileUpload>(result.Value);
    }

    /// <summary>
    /// Update a file upload's properties
    /// </summary>
    /// <param name="fileUploadId">ID of the file upload to update</param>
    /// <param name="request">Update file upload request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated file upload details</returns>
    [HttpPut("file-upload/{fileUploadId}")]
    public async Task<ActionResult<ApiResponseBase<FileUpload>>> UpdateFileUpload(
        [FromBody] UpdateFileUploadRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating file upload with ID: {FileUploadId}", request.FileUploadId);
        var command = mapper.Map<UpdateFileUploadCommand>(request);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<FileUpload>(result.Value);
    }

    /// <summary>
    /// Delete a file upload
    /// </summary>
    /// <param name="fileUploadId">ID of the file upload to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Operation result</returns>
    [HttpDelete("file-upload/{fileUploadId}")]
    public async Task<ActionResult<ApiResponseBase<bool>>> DeleteFileUpload(int fileUploadId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting file upload with ID: {FileUploadId}", fileUploadId);
        var command = new DeleteFileUploadCommand { FileUploadId = fileUploadId };
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<bool>(true);
    }

    #endregion

    #region FileDataMapping

    /// <summary>
    /// Create a new mapping between a file and a financial data point
    /// </summary>
    /// <param name="request">File mapping request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created mapping details</returns>
    [HttpPost("file-mapping")]
    public async Task<ActionResult<ApiResponseBase<FileDataMapping>>> CreateFileMapping(
        [FromBody] CreateFileMappingRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating file mapping for FileUpload: {FileUploadId} and DataPoint: {DataPointId}",
            request.FileUploadId, request.FinancialDataPointId);

        var command = mapper.Map<CreateFileMappingCommand>(request);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<FileDataMapping>(result.Value);
    }

    /// <summary>
    /// Get a specific file mapping by ID
    /// </summary>
    /// <param name="mappingId">ID of the mapping</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Mapping details</returns>
    [HttpGet("file-mapping/{mappingId}")]
    public async Task<ActionResult<ApiResponseBase<HillMetrics.Normalized.Domain.Contracts.AI.Dataset.FileDataMapping>>> GetFileMapping(
        int mappingId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting file mapping with ID: {MappingId}", mappingId);
        var query = new GetFileMappingQuery { MappingId = mappingId };
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<HillMetrics.Normalized.Domain.Contracts.AI.Dataset.FileDataMapping>(result.Value);
    }

    /// <summary>
    /// Get all mappings associated with a specific file upload
    /// </summary>
    /// <param name="fileUploadId">ID of the file upload</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of mappings</returns>
    [HttpGet("file-mappings/by-file-upload/{fileUploadId}")]
    public async Task<ActionResult<ApiResponseBase<List<HillMetrics.Normalized.Domain.Contracts.AI.Dataset.FileDataMapping>>>> GetMappingsByFileUpload(
        int fileUploadId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting mappings for file upload ID: {FileUploadId}", fileUploadId);
        var query = new GetMappingsByFileUploadQuery { FileUploadId = fileUploadId };
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<List<HillMetrics.Normalized.Domain.Contracts.AI.Dataset.FileDataMapping>>(result.Value);
    }

    /// <summary>
    /// Delete a file mapping
    /// </summary>
    /// <param name="mappingId">ID of the mapping to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Operation result</returns>
    [HttpDelete("file-mapping/{mappingId}")]
    public async Task<ActionResult<ApiResponseBase<bool>>> DeleteFileMapping(int mappingId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting file mapping with ID: {MappingId}", mappingId);
        var command = new DeleteFileMappingCommand { MappingId = mappingId };
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<bool>(true);
    }

    #endregion

    #region ElementValue

    /// <summary>
    /// Create a new element value
    /// </summary>
    /// <param name="request">Element value request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created element value</returns>
    [HttpPost("element-value")]
    public async Task<ActionResult<ApiResponseBase<FileDataElementValue>>> CreateElementValue(
        [FromBody] CreateElementValuesRequest request,
        CancellationToken cancellationToken)
    {
        //logger.LogInformation("Creating element value for mapping ID: {MappingId}", request.FileDataMappingId);
        var command = mapper.Map<CreateElementValueCommand>(request);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<FileDataElementValue>(result.Value);
    }

    /// <summary>
    /// Create multiple element values
    /// </summary>
    /// <param name="request">Collection of element values request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Operation result</returns>
    [HttpPost("element-values")]
    public async Task<ActionResult<ApiResponseBase<bool>>> CreateElementValues(
        [FromBody] CreateElementValuesRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating multiple element values");
        var command = mapper.Map<CreateElementValuesCommand>(request);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<bool>(true);
    }

    /// <summary>
    /// Get all element values for a specific mapping
    /// </summary>
    /// <param name="mappingId">ID of the mapping</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of element values</returns>
    [HttpGet("element-values/by-mapping/{mappingId}")]
    public async Task<ActionResult<ApiResponseBase<List<FileDataElementValue>>>> GetElementValuesByMapping(
        int mappingId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting element values for mapping ID: {MappingId}", mappingId);
        var query = new GetElementValuesByMappingQuery { MappingId = mappingId };
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<List<FileDataElementValue>>(result.Value);
    }

    /// <summary>
    /// Delete all element values for a specific mapping
    /// </summary>
    /// <param name="mappingId">ID of the mapping</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Operation result</returns>
    [HttpDelete("element-values/by-mapping/{mappingId}")]
    public async Task<ActionResult<ApiResponseBase<bool>>> DeleteElementValuesByMapping(
        int mappingId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting element values for mapping ID: {MappingId}", mappingId);
        var command = new DeleteElementValuesByMappingCommand { MappingId = mappingId };
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<bool>(true);
    }

    #endregion

    #region FinancialDataPoint

    /// <summary>
    /// Create a new financial data point
    /// </summary>
    /// <param name="request">Financial data point request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created financial data point</returns>
    [HttpPost("financial-data-point")]
    public async Task<ActionResult<ApiResponseBase<FinancialDataPoint>>> CreateFinancialDataPoint(
        [FromBody] CreateFinancialDataPointRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating financial data point with name: {Name}", request.Name);
        var command = mapper.Map<CreateFinancialDataPointCommand>(request);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<FinancialDataPoint>(result.Value);
    }

    /// <summary>
    /// Get all financial data points
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of financial data points</returns>
    [HttpGet("financial-data-points")]
    public async Task<ActionResult<ApiResponseBase<List<FinancialDataPoint>>>> GetAllFinancialDataPoints(CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all financial data points");
        var query = new GetAllFinancialDataPointsQuery();
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<List<FinancialDataPoint>>(result.Value);
    }

    /// <summary>
    /// Get a specific financial data point by ID
    /// </summary>
    /// <param name="dataPointId">ID of the data point</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Financial data point details</returns>
    [HttpGet("financial-data-point/{dataPointId}")]
    public async Task<ActionResult<ApiResponseBase<FinancialDataPoint>>> GetFinancialDataPoint(
        int dataPointId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting financial data point with ID: {DataPointId}", dataPointId);
        var query = new GetFinancialDataPointQuery { DataPointId = dataPointId };
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<FinancialDataPoint>(result.Value);
    }

    /// <summary>
    /// Update a financial data point
    /// </summary>
    /// <param name="dataPointId">ID of the data point to update</param>
    /// <param name="command">Update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated financial data point</returns>
    [HttpPut("financial-data-point/{dataPointId}")]
    public async Task<ActionResult<ApiResponseBase<FinancialDataPoint>>> UpdateFinancialDataPoint(
        int dataPointId,
        [FromBody] CreateFinancialDataPointRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating financial data point with ID: {DataPointId}", dataPointId);

        var command = mapper.Map<UpdateFinancialDataPointCommand>(request);
        command.DataPointId = dataPointId;

        var result = await mediator.Send(mapper.Map<UpdateFinancialDataPointCommand>(request), cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<FinancialDataPoint>(result.Value);
    }

    /// <summary>
    /// Delete a financial data point
    /// </summary>
    /// <param name="dataPointId">ID of the data point to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Operation result</returns>
    [HttpDelete("financial-data-point/{dataPointId}")]
    public async Task<ActionResult<ApiResponseBase<bool>>> DeleteFinancialDataPoint(
        int dataPointId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting financial data point with ID: {DataPointId}", dataPointId);
        var command = new DeleteFinancialDataPointCommand { DataPointId = dataPointId };
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<bool>(true);
    }

    #endregion

    #region FinancialDataPointElement

    /// <summary>
    /// Create a new financial data point element
    /// </summary>
    /// <param name="command">Element details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created data point element</returns>
    [HttpPost("data-point-element")]
    public async Task<ActionResult<ApiResponseBase<FinancialDataPointElement>>> CreateDataPointElement(
        [FromBody] CreateDataPointElementCommand command,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating element for data point ID: {DataPointId}", command.FinancialDataPointId);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<FinancialDataPointElement>(result.Value);
    }

    /// <summary>
    /// Create multiple financial data point elements
    /// </summary>
    /// <param name="command">Collection of elements to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Operation result</returns>
    [HttpPost("data-point-elements")]
    public async Task<ActionResult<ApiResponseBase<bool>>> CreateDataPointElements(
        [FromBody] CreateDataPointElementsCommand command,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating multiple data point elements");
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<bool>(true);
    }

    /// <summary>
    /// Get all elements for a specific data point
    /// </summary>
    /// <param name="dataPointId">ID of the data point</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of data point elements</returns>
    [HttpGet("data-point-elements/by-data-point/{dataPointId}")]
    public async Task<ActionResult<ApiResponseBase<List<FinancialDataPointElement>>>> GetElementsByDataPoint(
        int dataPointId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting elements for data point ID: {DataPointId}", dataPointId);
        var query = new GetElementsByDataPointQuery { DataPointId = dataPointId };
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<List<FinancialDataPointElement>>(result.Value);
    }

    /// <summary>
    /// Delete all elements for a specific data point
    /// </summary>
    /// <param name="dataPointId">ID of the data point</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Operation result</returns>
    [HttpDelete("data-point-elements/by-data-point/{dataPointId}")]
    public async Task<ActionResult<ApiResponseBase<bool>>> DeleteElementsByDataPoint(
        int dataPointId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting elements for data point ID: {DataPointId}", dataPointId);
        var command = new DeleteElementsByDataPointCommand { DataPointId = dataPointId };
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<bool>(true);
    }

    #endregion
}
