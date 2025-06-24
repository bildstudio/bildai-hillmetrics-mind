using AutoMapper;
using HillMetrics.Core.API.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset.Cqrs.ElementValue;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset.Cqrs.FileDataMapping;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;
using HillMetrics.MIND.API.Contracts.Responses;
using HillMetrics.MIND.API.Contracts.Requests.AiDataset;
using HillMetrics.Core.Financial;
using Microsoft.AspNetCore.Authorization;
using HillMetrics.MIND.API.Contracts.Responses.AiDataset;
using HillMetrics.Core.Mediator;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset.Cqrs.FileUpload;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset.Cqrs.FinancialDataPoint;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset.Cqrs.PropertyDataType;
using HillMetrics.MIND.API.Contracts.Responses.AiDataset.Metadatas;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset.FinancialDataPointMetadatas;
using HillMetrics.MIND.API.Mappers;
using HillMetrics.MIND.API.Contracts.Requests.AiDataset.Metadatas;
using HillMetrics.MIND.API.Contracts.Responses.Common;
using HillMetrics.Core.Financial.DataPoint;
using HillMetrics.Normalized.Domain.Contracts.Market.Cqrs.Rule;
using HillMetrics.Core.Rules;
using FluentResults;
using HillMetrics.Core.Contracts;
using HillMetrics.Core.Errors;
using HillMetrics.Normalized.Infrastructure.Contracts.Database.Entity;
using HillMetrics.Normalized.Infrastructure.Database.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HillMetrics.MIND.API.Controllers;

[Route("api/v{v:apiVersion}/[controller]")]
public class FluxCaracController(IHMediator mediator, IMapper mapper, ILogger<FluxCaracController> logger) : BaseHillMetricsController(mediator)
{
    #region FileUpload
    [HttpGet("file-uploads/search")]
    public async Task<ActionResult<CustomMindPagedApiResponseBase<FileUploadSearchResponse>>> SearchFileUpload(
    [FromQuery] FileUploadSearchRequest request,
    CancellationToken cancellationToken)
    {
        logger.LogInformation("Searching file uploads with criteria: {Criteria}",
            System.Text.Json.JsonSerializer.Serialize(request));

        var query = mapper.Map<SearchFileUploadQuery>(request);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new CustomMindPagedApiResponseBase<FileUploadSearchResponse>(
            mapper.Map<List<FileUploadSearchResponse>>(result.Value.Results),
            result.Value.TotalRecords);
    }

    /// <summary>
    /// Upload a new file for AI dataset processing
    /// </summary>
    /// <param name="file">File upload</param>
    /// <param name="difficulty">Difficulty</param>
    /// <param name="financialType">Financial type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Details of the uploaded file</returns>
    [HttpPost("file-upload")]
    public async Task<ActionResult<ApiResponseBase<FileUpload>>> CreateFileUpload(
        IFormFile file,
        FileDifficulty difficulty,
        FinancialType financialType,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling file upload request for file: {FileName}", file.FileName);

        using var stream = file.OpenReadStream();
        var command = new CreateFileUploadCommand
        {
            FileName = file.FileName,
            ContentType = Core.Common.ContentTypeMapper.GetContentType(file.ContentType),
            FileStream = stream,
            FileMetadataId = 0,
            FluxFetchingContentId = null,
            Difficulty = difficulty,
            MappingStatus = MappingStatus.NotMapped,
            FinancialType = financialType
        };

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<FileUpload>(result.Value);
    }

    /// <summary>
    /// Create a file upload from a flux content
    /// </summary>
    /// <param name="request">File upload request with flux content reference</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Details of the uploaded file</returns>
    [HttpPost("file-upload/from-flux")]
    public async Task<ActionResult<ApiResponseBase<FileUpload>>> CreateFileUploadFromFlux(
        [FromBody] CreateFileUploadFromFluxRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating file upload from flux content: {FluxFetchingContentId}", request.FluxFetchingContentId);

        var command = new CreateFileUploadCommand
        {
            FileName = request.FileName,
            ContentType = Core.Common.ContentTypeMapper.GetContentType(request.ContentType),
            FluxFetchingContentId = request.FluxFetchingContentId,
            Difficulty = request.Difficulty,
            MappingStatus = MappingStatus.NotMapped,
            FinancialType = request.FinancialType
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
        int fileUploadId,
        [FromBody] UpdateFileUploadRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating file upload with ID: {FileUploadId}", fileUploadId);

        var command = new CreateFileUploadCommand
        {
            FileUploadId = fileUploadId,
            FileName = request.FileName,
            ContentType = Core.Common.ContentType.Unknown,
            Difficulty = request.Difficulty,
            MappingStatus = request.MappingStatus,
            FinancialType = request.FinancialType
        };

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
    public async Task<ActionResult<ApiResponseBase<FileDataMapping>>> GetFileMapping(
        int mappingId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting file mapping with ID: {MappingId}", mappingId);
        var query = new GetFileMappingQuery { MappingId = mappingId };
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<FileDataMapping>(result.Value);
    }

    /// <summary>
    /// Get all mappings associated with a specific file upload
    /// </summary>
    /// <param name="fileUploadId">ID of the file upload</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of mappings</returns>
    [HttpGet("file-mappings/by-file-upload/{fileUploadId}")]
    public async Task<ActionResult<ApiResponseBase<List<FileDataMapping>>>> GetMappingsByFileUpload(
        int fileUploadId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting mappings for file upload ID: {FileUploadId}", fileUploadId);
        var query = new GetMappingsByFileUploadQuery { FileUploadId = fileUploadId };
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<List<FileDataMapping>>(result.Value);
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
        var command = new CreateFinancialDataPointCommand()
        {
            FinancialDataPoint = new FinancialDataPoint()
            {
                Id = 0,
                Name = request.Name,
                Description = request.Description,
                FinancialType = request.FinancialType,
                Elements = request.Elements.Select(x => new FinancialDataPointElement()
                {
                    PropertyName = x.PropertyName,
                    Description = x.Description,
                    Id = x.Id,
                    FinancialDataPointId = 0,
                    Position = x.Position,
                    PotentialValues = x.PotentialValues,
                    MappingPrimitiveValue = x.MappingPrimitiveValue,
                    Commentary = x.Commentary,
                    FinancialTechnicalDataPoint = x.FinancialTechnicalDataPoint,
                    ExternalName = x.ExternalName,
                    ParentElementId = x.ParentElementId
                }).ToList()
            }
        };

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<FinancialDataPoint>(result.Value);
    }

    /// <summary>
    /// Update a financial data point
    /// </summary>
    /// <param name="dataPointId">ID of the data point to update</param>
    /// <param name="request">Update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated financial data point</returns>
    [HttpPut("financial-data-point/{dataPointId}")]
    public async Task<ActionResult<ApiResponseBase<FinancialDataPoint>>> UpdateFinancialDataPoint(
        int dataPointId,
        [FromBody] CreateFinancialDataPointRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating financial data point with ID: {DataPointId}, name: {Name}",
            dataPointId, request.Name);

        var command = new CreateFinancialDataPointCommand()
        {
            FinancialDataPoint = new FinancialDataPoint()
            {
                Id = dataPointId,
                Name = request.Name,
                Description = request.Description,
                FinancialType = request.FinancialType,
                Elements = request.Elements.Select(x => new FinancialDataPointElement()
                {
                    PropertyName = x.PropertyName,
                    Description = x.Description,
                    Id = x.Id,
                    FinancialDataPointId = dataPointId,
                    Position = x.Position,
                    PotentialValues = x.PotentialValues,
                    MappingPrimitiveValue = x.MappingPrimitiveValue,
                    ExternalName = x.ExternalName,
                    Commentary = x.Commentary,
                    FinancialTechnicalDataPoint = x.FinancialTechnicalDataPoint,
                    ParentElementId = x.ParentElementId
                }).ToList()
            }
        };

        var result = await mediator.Send(command, cancellationToken);

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

    /// <summary>
    /// Search for financial data points following the given criteria
    /// </summary>
    /// <param name="request">Search criteria</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paged list of financial data points</returns>
    [HttpGet("financial-data-points/search")]
    public async Task<ActionResult<CustomMindPagedApiResponseBase<FinancialDataPointSearchResponse>>> SearchFinancialDataPoints(
        [FromQuery] SearchFinancialDataPointRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Searching financial data points with criteria: {Criteria}",
            System.Text.Json.JsonSerializer.Serialize(request));

        var query = mapper.Map<SearchFinancialDataPointQuery>(request);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new CustomMindPagedApiResponseBase<FinancialDataPointSearchResponse>(
            mapper.Map<List<FinancialDataPointSearchResponse>>(result.Value.Results),
            result.Value.TotalRecords);
    }

    /// <summary>
    /// Get financial data points by document type with metadata information
    /// </summary>
    /// <param name="documentTypeId">ID of the document type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Financial data points with metadata for the specified document type</returns>
    [HttpGet("financial-data-points/by-document-type/{documentTypeId}")]
    public async Task<ActionResult<ApiResponseBase<GetFinancialDataPointsByDocumentTypeQueryResult>>> GetFinancialDataPointsByDocumentType(
        int documentTypeId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting financial data points for document type: {DocumentTypeId}", documentTypeId);

        var query = new GetFinancialDataPointsByDocumentTypeQuery { DocumentTypeId = documentTypeId };
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<GetFinancialDataPointsByDocumentTypeQueryResult>(result.Value);
    }

    #endregion

    #region FinancialDataPointElement

    //search
    [HttpGet("financial-data-point-element/metadata/search")]
    public async Task<ActionResult<ListMetadatasResponse>> ListAsync(
        [FromQuery] int elementId,
        [FromQuery] int? documentTypeId,
        [FromQuery] int? languaged,
        [FromQuery] FinancialDataPointElementMetadataKey? key)
    {
        var query = new SearchMetadataQuery(elementId, documentTypeId, languaged, key);

        var result = await mediator.Send(query);
        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        List<FinancialDataPointElementMetadataDto> dtos = result.Value.FromDomainsList();
        return new ListMetadatasResponse(dtos, dtos.Count);
    }
    //post
    [HttpPost("financial-data-point-element/metadata")]
    public async Task<ActionResult<GetMetadataResponse>> CreateAsync([FromBody] SaveMetadataRequest request)
    {
        var command = new CreateMetadataCommand(
            new MetadataModel(request.FinancialDataPointElementId, request.DocumentTypeId, request.LanguageId, request.Values));

        var result = await mediator.Send(command);
        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        FinancialDataPointElementMetadataDto dto = result.Value.FromDomain();
        return new GetMetadataResponse(dto);
    }

    [HttpPut("financial-data-point-element/metadata")]
    public async Task<ActionResult<GetMetadataResponse>> UpdateAsync([FromBody] SaveMetadataRequest request)
    {
        var command = new UpdateMetadataCommand(
            new MetadataModel(request.FinancialDataPointElementId, request.DocumentTypeId, request.LanguageId, request.Values));

        var result = await mediator.Send(command);
        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        FinancialDataPointElementMetadataDto dto = result.Value.FromDomain();
        return new GetMetadataResponse(dto);
    }

    //delete
    [HttpDelete("financial-data-point-element/metadata")]
    public async Task<ActionResult<DeletedResponse>> DeleteAsync([FromBody] DeleteMetadataRequest request)
    {
        var command = new DeleteMetadataCommand(request.FinancialDataPointElementId, request.DocumentTypeId, request.LanguageId, request.KeysToDelete);

        var result = await mediator.Send(command);
        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new DeletedResponse("Metadata deleted.");
    }


    #endregion

    #region PropertyDataType

    /// <summary>
    /// Create a new property data type
    /// </summary>
    [HttpPost("property-mapping")]
    public async Task<ActionResult<ApiResponseBase<PropertyMappingResponse>>> CreatePropertyDataType(
        [FromBody] CreatePropertyDataTypeRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating property data type with name: {Name}", request.Name);

        var command = new CreatePropertyMappingCommand
        {
            PropertyDataType = new PropertyMapping
            {
                Name = request.Name,
                Description = request.Description,
                ContentType = request.ContentType,
                MappingSourceType = request.MappingSourceType
            }
        };

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<PropertyMappingResponse>(mapper.Map<PropertyMappingResponse>(result.Value));
    }

    /// <summary>
    /// Update an existing property data type
    /// </summary>
    [HttpPut("property-mapping/{id}")]
    public async Task<ActionResult<ApiResponseBase<PropertyMappingResponse>>> UpdatePropertyDataType(
        int id,
        [FromBody] CreatePropertyDataTypeRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating property data type with ID: {Id}", id);

        var command = new CreatePropertyMappingCommand
        {
            PropertyDataType = new PropertyMapping
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                ContentType = request.ContentType,
                MappingSourceType = request.MappingSourceType
            }
        };

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<PropertyMappingResponse>(mapper.Map<PropertyMappingResponse>(result.Value));
    }

    /// <summary>
    /// Get a specific property data type
    /// </summary>
    [HttpGet("property-mapping/{id}")]
    public async Task<ActionResult<ApiResponseBase<PropertyMappingResponse>>> GetPropertyDataType(
        int id,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting property data type with ID: {Id}", id);

        var query = new GetPropertyMappingQuery { PropertyDataTypeId = id };
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<PropertyMappingResponse>(mapper.Map<PropertyMappingResponse>(result.Value));
    }

    /// <summary>
    /// Delete a property data type
    /// </summary>
    [HttpDelete("property-mapping/{id}")]
    public async Task<ActionResult<ApiResponseBase<bool>>> DeletePropertyDataType(
        int id,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting property data type with ID: {Id}", id);

        var command = new DeletePropertyMappingCommand { PropertyDataTypeId = id };
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<bool>(true);
    }

    /// <summary>
    /// Search property data types
    /// </summary>
    ///
    [Authorize]
    [HttpGet("property-mapping/search")]
    public async Task<ActionResult<CustomMindPagedApiResponseBase<PropertyMappingResponse>>> SearchPropertyDataTypes(
        [FromQuery] SearchPropertyDataTypeRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Searching property data types");

        var query = mapper.Map<SearchPropertyMappingQuery>(request);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        var res = new CustomMindPagedApiResponseBase<PropertyMappingResponse>(
            mapper.Map<List<PropertyMappingResponse>>(result.Value.Results),
            result.Value.TotalRecords);

        var json = System.Text.Json.JsonSerializer.Serialize(res);
        return res;
    }

    #endregion

    #region FinancialRules

    /// <summary>
    /// Search for financial rules based on a specific data point or get all rules
    /// </summary>
    /// <param name="dataPoint">Optional financial technical data point to filter rules</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Financial rules with markdown documentation</returns>
    [HttpGet("financial-rules")]
    public async Task<ActionResult<ApiResponseBase<SearchFinancialRuleQueryResult>>> SearchFinancialRules(
        [FromQuery] FinancialTechnicalDataPoint? dataPoint = null,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Searching financial rules for data point: {DataPoint}",
            dataPoint.HasValue ? dataPoint.Value.ToString() : "All");

        var query = new SearchFinancialRuleQuery { DataPoint = dataPoint };
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
            return new ErrorApiActionResult(result.Errors.ToApiResult());

        return new ApiResponseBase<SearchFinancialRuleQueryResult>(result.Value);
    }

    #endregion
}
