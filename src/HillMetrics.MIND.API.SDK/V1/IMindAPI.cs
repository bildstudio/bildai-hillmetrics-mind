using HillMetrics.Core.Financial;
using HillMetrics.MIND.API.Contracts.Requests.AiDataset;
using HillMetrics.MIND.API.Contracts.Requests.Flux;
using HillMetrics.MIND.API.Contracts.Requests.Prices;
using HillMetrics.MIND.API.Contracts.Requests.Source;
using HillMetrics.MIND.API.Contracts.Responses;
using HillMetrics.MIND.API.Contracts.Responses.AiDataset;
using HillMetrics.MIND.API.Contracts.Responses.Flux;
using HillMetrics.MIND.API.Contracts.Responses.Prices;
using HillMetrics.MIND.API.Contracts.Responses.Source;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset.Cqrs.ElementValue;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset.Cqrs.FinancialDataPoint;
using Refit;

namespace HillMetrics.MIND.API.SDK.V1
{
    /// <summary>
    /// Interface Refit pour l'API MIND - expose les endpoints du FluxController
    /// </summary>
    public interface IMindAPI
    {
        #region Flux Management

        /// <summary>
        /// Search for fluxes following the given criteria
        /// </summary>
        [Get("/api/v1/flux/search")]
        Task<PagedApiResponseBase<FluxSearchResponse>> SearchFluxAsync([Query] FluxSearchRequest request);

        /// <summary>
        /// Get the details of a flux
        /// </summary>
        [Get("/api/v1/flux/{id}")]
        Task<FluxResponseWrapper> GetFluxAsync(int id);

        /// <summary>
        /// Create a new flux
        /// </summary>
        [Post("/api/v1/flux")]
        Task<bool> CreateFluxAsync([Body] FluxRequest request);

        /// <summary>
        /// Update a flux
        /// </summary>
        [Put("/api/v1/flux")]
        Task<bool> UpdateFluxAsync(int fluxId, [Body] FluxRequest request);

        /// <summary>
        /// Delete a flux
        /// </summary>
        [Delete("/api/v1/flux")]
        Task<bool> DeleteFluxAsync(int fluxId);

        /// <summary>
        /// Force the fetch of a flux
        /// </summary>
        [Get("/api/v1/flux/{id}/force-fetch")]
        Task<FluxForceFetchResponse> ForceFetchAsync(int id);

        /// <summary>
        /// Force the process of a flux
        /// </summary>
        [Get("/api/v1/flux/{id}/force-process")]
        Task<FluxForceProcessResponse> ForceProcessAsync(int id);

        /// <summary>
        /// Force the processing of normalized financial prices for multiple financial IDs
        /// </summary>
        [Post("/api/v1/flux/force-financial-price-processing")]
        Task<ApiResponseBase<ForceFinancialPriceProcessingResponse>> ForceFinancialPriceProcessingAsync([Body] ForceFinancialPriceProcessingRequest request);

        #endregion

        #region Financial Data Point

        /// <summary>
        /// Identify automatically the list of data points of the flux.
        /// </summary>
        [Get("/api/v1/flux/{id}/financial-data-point/identify")]
        Task<FluxIdentificationResponse> IdentifyDataPointAsync(int id);

        /// <summary>
        /// Identify automatically the list of data points of a flux fetching history id.
        /// </summary>
        [Get("/api/v1/flux/financial-data-point/identify/{fluxFetchingId}")]
        Task<FluxIdentificationResponse> IdentifyDataPointFromSpecificHistoryAsync(int fluxFetchingId);

        /// <summary>
        /// Validate (or invalidate) a financial data point
        /// </summary>
        [Post("/api/v1/flux/financial-data-point/validate")]
        Task<bool> ValidateFinancialDataPointAsync([Body] FluxValidateFinancialDataPointRequest request);

        #endregion

        #region Fetching

        /// <summary>
        /// Search for flux fetching history following the given criteria
        /// </summary>
        [Get("/api/v1/flux/fetching-history/search")]
        Task<PagedApiResponseBase<FluxFetchingSearchResponse>> SearchFetchingHistoryAsync([Query] FluxFetchingSearchRequest request);

        /// <summary>
        /// Get the details of a specific fetching history
        /// </summary>
        [Get("/api/v1/flux/fetching-history/{fetchingHistoryId}")]
        Task<ApiResponseBase<FluxFetchingResponse>> GetFetchingHistoryAsync(int fetchingHistoryId);

        /// <summary>
        /// Delete a specific fetching history
        /// </summary>
        /// <param name="fetchingHistoryId">ID of the fetching history to delete</param>
        /// <returns>API response indicating success or failure</returns>
        [Delete("/api/v1/flux/fetching-history/{fetchingHistoryId}")]
        Task<ApiResponseBase<bool>> DeleteFetchingHistoryAsync(int fetchingHistoryId);

        /// <summary>
        /// Get the details of a specific fetching content
        /// </summary>
        /// <param name="fetchingContentId">The fetching content identifier</param>
        /// <returns>Details of the fetching content</returns>
        [Get("/api/v1/flux/fetching-history/content/{fetchingContentId}")]
        Task<ApiResponseBase<FluxFetchingContentHistoryResponse>> GetFetchingContentAsync(int fetchingContentId);

        #endregion

        #region Processing

        /// <summary>
        /// Search for flux processing history following the given criteria
        /// </summary>
        [Get("/api/v1/flux/processing-history/search")]
        Task<PagedApiResponseBase<FluxProcessingSearchReponse>> SearchProcessingHistoryAsync([Query] FluxProcessingSearchRequest request);

        /// <summary>
        /// Get the details of a specific processing history
        /// </summary>
        [Get("/api/v1/flux/processing-history/{processingHistoryId}")]
        Task<ApiResponseBase<FluxProcessingResponse>> GetProcessingHistoryAsync(int processingHistoryId);

        #endregion

        #region Flux Errors

        /// <summary>
        /// Search for flux errors history following the given criteria
        /// </summary>
        [Get("/api/v1/flux/errors/search")]
        Task<PagedApiResponseBase<FluxErrorSearchResponse>> SearchFluxErrorsAsync([Query] FluxErrorSearchRequest request);

        /// <summary>
        /// Get the details of a specific error
        /// </summary>
        [Get("/api/v1/flux/errors/{errorId}")]
        Task<ApiResponseBase<FluxErrorResponse>> GetErrorAsync(int errorId);

        /// <summary>
        /// Delete multiple flux errors
        /// </summary>
        /// <param name="errorIds">List of error IDs to delete</param>
        /// <returns>API response indicating success or failure</returns>
        [Delete("/api/v1/flux/errors")]
        Task<ApiResponseBase<bool>> DeleteFluxErrorsAsync([Body] List<int> errorIds);

        #endregion

        #region Sources
        /// <summary>
        /// Search for flux sources following the given criteria
        /// </summary>
        [Get("/api/v1/source/search")]
        Task<SourceSearchResponse> SearchSourcesAsync([Query] SourceSearchRequest request);

        /// <summary>
        /// Create a new source
        /// </summary>
        [Post("/api/v1/source")]
        Task<SourceResponse> CreateSourceAsync([Body] SourceCreateRequest request);

        /// <summary>
        /// Update a source
        /// </summary>
        [Put("/api/v1/source")]
        Task<SourceResponse> UpdateSourceAsync(int sourceId, [Body] SourceCreateRequest request);
        #endregion

        #region Workflow

        /// <summary>
        /// Gets the current state of all active workflow fluxes
        /// </summary>
        [Get("/api/v1/flux/workflow/active")]
        Task<ApiResponseBase<List<ActiveFluxDto>>> GetActiveFluxesAsync();

        /// <summary>
        /// Gets the state of recently completed fluxes
        /// </summary>
        [Get("/api/v1/flux/workflow/completed")]
        Task<ApiResponseBase<List<CompletedFluxDto>>> GetCompletedFluxesAsync([Query] int count = 10);

        /// <summary>
        /// Gets the details of a specific flux workflow
        /// </summary>
        [Get("/api/v1/flux/workflow/{fluxId}")]
        Task<ApiResponseBase<FluxWorkflowDetailsDto>> GetFluxWorkflowDetailsAsync(int fluxId);

        /// <summary>
        /// Gets a global summary of flux workflows
        /// </summary>
        [Get("/api/v1/flux/workflow/summary")]
        Task<ApiResponseBase<WorkflowSummaryDto>> GetFluxWorkflowSummaryAsync();

        /// <summary>
        /// Manually triggers cleanup of historical workflow data
        /// </summary>
        [Post("/api/v1/flux/workflow/cleanup")]
        Task<ApiResponseBase<string>> CleanupWorkflowHistoryAsync([Query] int daysToKeep = 14);

        #endregion

        #region Raw database
        /// <summary>
        /// Get raw content file as a stream
        /// </summary>
        /// <param name="id">ID of the raw content in MongoDB</param>
        /// <returns>Stream of the raw content file</returns>
        [Get("/api/v1/raw/{id}")]
        Task<Stream> GetFile(string id);

        /// <summary>
        /// Download a file from FileMetadataRepository using its string ID
        /// </summary>
        /// <param name="id">File metadata string ID</param>
        /// <returns>Stream of the file content</returns>
        [Get("/api/v1/raw/stored-file/{id}")]
        Task<Stream> DownloadStoredFile(int id);
        #endregion

        #region AI Dataset - File Upload

        /// <summary>
        /// Create a new file upload from a direct file upload
        /// </summary>
        /// <param name="file"></param>
        /// <param name="difficulty"></param>
        /// <param name="financialType"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Multipart]
        [Post("/api/v1/fluxcarac/file-upload")]
        Task<ApiResponseBase<FileUpload>> CreateFileUploadAsync(
            [AliasAs("file")] StreamPart file,
            [AliasAs("difficulty")] FileDifficulty difficulty = FileDifficulty.Medium,
            [AliasAs("financialType")] FinancialType financialType = FinancialType.Undefined,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a file upload from a flux content
        /// </summary>
        [Post("/api/v1/fluxcarac/file-upload/from-flux")]
        Task<ApiResponseBase<FileUpload>> CreateFileUploadFromFluxAsync(
            [Body] CreateFileUploadFromFluxRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all uploaded files
        /// </summary>
        [Get("/api/v1/fluxcarac/file-uploads")]
        Task<ApiResponseBase<List<FileUpload>>> GetAllFileUploadsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get details of a specific file upload
        /// </summary>
        [Get("/api/v1/fluxcarac/file-upload/{fileUploadId}")]
        Task<ApiResponseBase<FileUpload>> GetFileUploadAsync(
            int fileUploadId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Update a file upload's properties
        /// </summary>
        [Put("/api/v1/fluxcarac/file-upload/{fileUploadId}")]
        Task<ApiResponseBase<FileUpload>> UpdateFileUploadAsync(
            int fileUploadId,
            [Body] UpdateFileUploadRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a file upload
        /// </summary>
        [Delete("/api/v1/fluxcarac/file-upload/{fileUploadId}")]
        Task<ApiResponseBase<bool>> DeleteFileUploadAsync(
            int fileUploadId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Update file difficulty
        /// </summary>
        [Put("/api/v1/fluxcarac/file-upload/{fileUploadId}/difficulty")]
        Task<ApiResponseBase<FileUpload>> UpdateFileUploadDifficultyAsync(
            int fileUploadId,
            [Body] FileDifficulty difficulty,
            CancellationToken cancellationToken = default);
        #endregion

        #region AI Dataset - File Data Mapping

        /// <summary>
        /// Create a new mapping between a file and a financial data point
        /// </summary>
        /// <param name="request">Mapping details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created mapping details</returns>
        [Post("/api/v1/fluxcarac/file-mapping")]
        Task<ApiResponseBase<HillMetrics.Normalized.Domain.Contracts.AI.Dataset.FileDataMapping>> CreateFileMappingAsync(
            [Body] CreateFileMappingRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a specific file mapping by ID
        /// </summary>
        /// <param name="mappingId">ID of the mapping</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Mapping details</returns>
        [Get("/api/v1/fluxcarac/file-mapping/{mappingId}")]
        Task<ApiResponseBase<HillMetrics.Normalized.Domain.Contracts.AI.Dataset.FileDataMapping>> GetFileMappingAsync(
            int mappingId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all mappings associated with a specific file upload
        /// </summary>
        /// <param name="fileUploadId">ID of the file upload</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of mappings</returns>
        [Get("/api/v1/fluxcarac/file-mappings/by-file-upload/{fileUploadId}")]
        Task<ApiResponseBase<List<HillMetrics.Normalized.Domain.Contracts.AI.Dataset.FileDataMapping>>> GetMappingsByFileUploadAsync(
            int fileUploadId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a file mapping
        /// </summary>
        /// <param name="mappingId">ID of the mapping to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Operation result</returns>
        [Delete("/api/v1/fluxcarac/file-mapping/{mappingId}")]
        Task<ApiResponseBase<bool>> DeleteFileMappingAsync(
            int mappingId,
            CancellationToken cancellationToken = default);

        #endregion

        #region AI Dataset - Element Value
        /// <summary>
        /// Get all element values for a specific mapping
        /// </summary>
        /// <param name="mappingId">ID of the mapping</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of element values</returns>
        [Get("/api/v1/fluxcarac/element-values/by-mapping/{mappingId}")]
        Task<ApiResponseBase<List<FileDataElementValue>>> GetElementValuesByMappingAsync(
            int mappingId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete all element values for a specific mapping
        /// </summary>
        /// <param name="mappingId">ID of the mapping</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Operation result</returns>
        [Delete("/api/v1/fluxcarac/element-values/by-mapping/{mappingId}")]
        Task<ApiResponseBase<bool>> DeleteElementValuesByMappingAsync(
            int mappingId,
            CancellationToken cancellationToken = default);

        #endregion

        #region AI Dataset - Financial Data Point

        /// <summary>
        /// Create a new financial data point
        /// </summary>
        /// <param name="request">Financial data point details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created financial data point</returns>
        [Post("/api/v1/fluxcarac/financial-data-point")]
        Task<ApiResponseBase<FinancialDataPoint>> CreateFinancialDataPointAsync(
            [Body] CreateFinancialDataPointRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all financial data points
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of financial data points</returns>
        [Get("/api/v1/fluxcarac/financial-data-points")]
        Task<ApiResponseBase<List<FinancialDataPoint>>> GetAllFinancialDataPointsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a specific financial data point by ID
        /// </summary>
        /// <param name="dataPointId">ID of the data point</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Financial data point details</returns>
        [Get("/api/v1/fluxcarac/financial-data-point/{dataPointId}")]
        Task<ApiResponseBase<FinancialDataPoint>> GetFinancialDataPointAsync(
            int dataPointId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Update a financial data point
        /// </summary>
        /// <param name="dataPointId">ID of the data point to update</param>
        /// <param name="request">Update data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated financial data point</returns>
        [Put("/api/v1/fluxcarac/financial-data-point/{dataPointId}")]
        Task<ApiResponseBase<FinancialDataPoint>> UpdateFinancialDataPointAsync(
            int dataPointId,
            [Body] CreateFinancialDataPointRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a financial data point
        /// </summary>
        /// <param name="dataPointId">ID of the data point to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Operation result</returns>
        [Delete("/api/v1/fluxcarac/financial-data-point/{dataPointId}")]
        Task<ApiResponseBase<bool>> DeleteFinancialDataPointAsync(
            int dataPointId,
            CancellationToken cancellationToken = default);

        #endregion

        #region AI Dataset - Financial Data Point Element

        #endregion

        #region AI Dataset - Property Data Type

        /// <summary>
        /// Create a new property data type
        /// </summary>
        [Post("/api/v1/fluxcarac/property-data-type")]
        Task<ApiResponseBase<PropertyDataTypeResponse>> CreatePropertyDataTypeAsync(
            [Body] CreatePropertyDataTypeRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Update an existing property data type
        /// </summary>
        [Put("/api/v1/fluxcarac/property-data-type/{id}")]
        Task<ApiResponseBase<PropertyDataTypeResponse>> UpdatePropertyDataTypeAsync(
            int id,
            [Body] CreatePropertyDataTypeRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a specific property data type
        /// </summary>
        [Get("/api/v1/fluxcarac/property-data-type/{id}")]
        Task<ApiResponseBase<PropertyDataTypeResponse>> GetPropertyDataTypeAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a property data type
        /// </summary>
        [Delete("/api/v1/fluxcarac/property-data-type/{id}")]
        Task<ApiResponseBase<bool>> DeletePropertyDataTypeAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Search property data types
        /// </summary>
        [Get("/api/v1/fluxcarac/property-data-types/search")]
        Task<PagedApiResponseBase<PropertyDataTypeResponse>> SearchPropertyDataTypesAsync(
            [Query] SearchPropertyDataTypeRequest request,
            CancellationToken cancellationToken = default);

        #endregion

        #region Prices

        /// <summary>
        /// Update a price entity
        /// </summary>
        [Post("/api/v1/prices/update")]
        Task<ApiResponseBase<bool>> UpdatePriceAsync([Body] UpdatePriceRequest request);

        /// <summary>
        /// Search for prices based on criteria
        /// </summary>
        [Get("/api/v1/prices/search")]
        Task<PagedApiResponseBase<SearchPricesResponse>> SearchPricesAsync([Query] SearchPricesRequest request);

        #endregion

        /// <summary>
        /// Search for financial data points based on criteria
        /// </summary>
        /// <param name="request">Search criteria</param>
        /// <returns>Paged list of financial data points</returns>
        [Get("/api/v1/fluxcarac/financial-data-points/search")]
        Task<PagedApiResponseBase<FinancialDataPointSearchResponse>> SearchFinancialDataPointsAsync([Query] SearchFinancialDataPointRequest request);
    }
}
