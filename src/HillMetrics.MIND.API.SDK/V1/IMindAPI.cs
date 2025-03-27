using HillMetrics.Core.Financial;
using HillMetrics.MIND.API.Contracts.Requests.AiDataset;
using HillMetrics.MIND.API.Contracts.Requests.Flux;
using HillMetrics.MIND.API.Contracts.Requests.Prices;
using HillMetrics.MIND.API.Contracts.Requests.Source;
using HillMetrics.MIND.API.Contracts.Responses;
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
        #endregion

        #region AI Dataset - File Upload

        /// <summary>
        /// Upload a new file for AI dataset processing
        /// </summary>
        /// <param name="file">File to upload</param>
        /// <param name="difficulty">Difficulty level of the file</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Details of the uploaded file</returns>
        [Multipart]
        [Post("/api/fluxcarac/file-upload")]
        Task<ApiResponseBase<FileUpload>> CreateFileUploadAsync(
            [AliasAs("file")] StreamPart file,
            [AliasAs("difficulty")] FileDifficulty difficulty = FileDifficulty.Medium,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all uploaded files
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of all file uploads</returns>
        [Get("/api/fluxcarac/file-uploads")]
        Task<ApiResponseBase<List<FileUpload>>> GetAllFileUploadsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get details of a specific file upload
        /// </summary>
        /// <param name="fileUploadId">ID of the file upload</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>File upload details</returns>
        [Get("/api/fluxcarac/file-upload/{fileUploadId}")]
        Task<ApiResponseBase<FileUpload>> GetFileUploadAsync(int fileUploadId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update a file upload's properties
        /// </summary>
        /// <param name="request">Update data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated file upload details</returns>
        [Put("/api/fluxcarac/file-upload/")]
        Task<ApiResponseBase<FileUpload>> UpdateFileUploadAsync(
            [Body] UpdateFileUploadRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a file upload
        /// </summary>
        /// <param name="fileUploadId">ID of the file upload to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Operation result</returns>
        [Delete("/api/fluxcarac/file-upload/{fileUploadId}")]
        Task<ApiResponseBase<bool>> DeleteFileUploadAsync(int fileUploadId, CancellationToken cancellationToken = default);

        #endregion

        #region AI Dataset - File Data Mapping

        /// <summary>
        /// Create a new mapping between a file and a financial data point
        /// </summary>
        /// <param name="request">Mapping details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created mapping details</returns>
        [Post("/api/fluxcarac/file-mapping")]
        Task<ApiResponseBase<HillMetrics.Normalized.Domain.Contracts.AI.Dataset.FileDataMapping>> CreateFileMappingAsync(
            [Body] CreateFileMappingRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a specific file mapping by ID
        /// </summary>
        /// <param name="mappingId">ID of the mapping</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Mapping details</returns>
        [Get("/api/fluxcarac/file-mapping/{mappingId}")]
        Task<ApiResponseBase<HillMetrics.Normalized.Domain.Contracts.AI.Dataset.FileDataMapping>> GetFileMappingAsync(
            int mappingId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all mappings associated with a specific file upload
        /// </summary>
        /// <param name="fileUploadId">ID of the file upload</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of mappings</returns>
        [Get("/api/fluxcarac/file-mappings/by-file-upload/{fileUploadId}")]
        Task<ApiResponseBase<List<HillMetrics.Normalized.Domain.Contracts.AI.Dataset.FileDataMapping>>> GetMappingsByFileUploadAsync(
            int fileUploadId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a file mapping
        /// </summary>
        /// <param name="mappingId">ID of the mapping to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Operation result</returns>
        [Delete("/api/fluxcarac/file-mapping/{mappingId}")]
        Task<ApiResponseBase<bool>> DeleteFileMappingAsync(
            int mappingId,
            CancellationToken cancellationToken = default);

        #endregion

        #region AI Dataset - Element Value

        /// <summary>
        /// Create a new element value
        /// </summary>
        /// <param name="command">Element value details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created element value</returns>
        [Post("/api/fluxcarac/element-value")]
        Task<ApiResponseBase<FileDataElementValue>> CreateElementValueAsync(
            [Body] CreateElementValueCommand command,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Create multiple element values
        /// </summary>
        /// <param name="command">Collection of element values</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Operation result</returns>
        [Post("/api/fluxcarac/element-values")]
        Task<ApiResponseBase<bool>> CreateElementValuesAsync(
            [Body] CreateElementValuesCommand command,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all element values for a specific mapping
        /// </summary>
        /// <param name="mappingId">ID of the mapping</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of element values</returns>
        [Get("/api/fluxcarac/element-values/by-mapping/{mappingId}")]
        Task<ApiResponseBase<List<FileDataElementValue>>> GetElementValuesByMappingAsync(
            int mappingId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete all element values for a specific mapping
        /// </summary>
        /// <param name="mappingId">ID of the mapping</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Operation result</returns>
        [Delete("/api/fluxcarac/element-values/by-mapping/{mappingId}")]
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
        [Post("/api/fluxcarac/financial-data-point")]
        Task<ApiResponseBase<FinancialDataPoint>> CreateFinancialDataPointAsync(
            [Body] CreateFinancialDataPointRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all financial data points
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of financial data points</returns>
        [Get("/api/fluxcarac/financial-data-points")]
        Task<ApiResponseBase<List<FinancialDataPoint>>> GetAllFinancialDataPointsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a specific financial data point by ID
        /// </summary>
        /// <param name="dataPointId">ID of the data point</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Financial data point details</returns>
        [Get("/api/fluxcarac/financial-data-point/{dataPointId}")]
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
        [Put("/api/fluxcarac/financial-data-point/{dataPointId}")]
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
        [Delete("/api/fluxcarac/financial-data-point/{dataPointId}")]
        Task<ApiResponseBase<bool>> DeleteFinancialDataPointAsync(
            int dataPointId,
            CancellationToken cancellationToken = default);

        #endregion

        #region AI Dataset - Financial Data Point Element

        /// <summary>
        /// Create a new financial data point element
        /// </summary>
        /// <param name="command">Element details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created data point element</returns>
        [Post("/api/fluxcarac/data-point-element")]
        Task<ApiResponseBase<FinancialDataPointElement>> CreateDataPointElementAsync(
            [Body] CreateDataPointElementCommand command,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Create multiple financial data point elements
        /// </summary>
        /// <param name="command">Collection of elements to create</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Operation result</returns>
        [Post("/api/fluxcarac/data-point-elements")]
        Task<ApiResponseBase<bool>> CreateDataPointElementsAsync(
            [Body] CreateDataPointElementsCommand command,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all elements for a specific data point
        /// </summary>
        /// <param name="dataPointId">ID of the data point</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of data point elements</returns>
        [Get("/api/fluxcarac/data-point-elements/by-data-point/{dataPointId}")]
        Task<ApiResponseBase<List<FinancialDataPointElement>>> GetElementsByDataPointAsync(
            int dataPointId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete all elements for a specific data point
        /// </summary>
        /// <param name="dataPointId">ID of the data point</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Operation result</returns>
        [Delete("/api/fluxcarac/data-point-elements/by-data-point/{dataPointId}")]
        Task<ApiResponseBase<bool>> DeleteElementsByDataPointAsync(
            int dataPointId,
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
        Task<ApiResponseBase<SearchPricesResponse>> SearchPricesAsync([Query] SearchPricesRequest request);

        #endregion
    }
}
