using HillMetrics.Core.Financial;
using HillMetrics.Core.Financial.DataPoint;
using HillMetrics.Core.Rules;
using HillMetrics.MIND.API.Contracts.Requests.AiDataset;
using HillMetrics.MIND.API.Contracts.Requests.AiDataset.DocumentTypes;
using HillMetrics.MIND.API.Contracts.Requests.AiDataset.Metadatas;
using HillMetrics.MIND.API.Contracts.Requests.Flux;
using HillMetrics.MIND.API.Contracts.Requests.Languages;
using HillMetrics.MIND.API.Contracts.Requests.Prices;
using HillMetrics.MIND.API.Contracts.Requests.Source;
using HillMetrics.MIND.API.Contracts.Requests.TradingVenue;
using HillMetrics.MIND.API.Contracts.Responses;
using HillMetrics.MIND.API.Contracts.Responses.AiDataset;
using HillMetrics.MIND.API.Contracts.Responses.AiDataset.DocumentTypes;
using HillMetrics.MIND.API.Contracts.Responses.AiDataset.Metadatas;
using HillMetrics.MIND.API.Contracts.Responses.Common;
using HillMetrics.MIND.API.Contracts.Responses.Flux;
using HillMetrics.MIND.API.Contracts.Responses.Languages;
using HillMetrics.MIND.API.Contracts.Responses.Prices;
using HillMetrics.MIND.API.Contracts.Responses.Source;
using HillMetrics.MIND.API.Contracts.Responses.TradingVenue;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset.Cqrs.ElementValue;
using HillMetrics.Normalized.Domain.Contracts.AI.Dataset.Cqrs.FinancialDataPoint;
using HillMetrics.Normalized.Domain.Contracts.Files;
using HillMetrics.Normalized.Domain.Contracts.Market.Cqrs.Rule;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux.Cqrs.Collect;
using Refit;

namespace HillMetrics.MIND.API.SDK.V1
{
    /// <summary>
    /// Interface Refit pour l'API MIND - expose les endpoints du FluxController
    /// </summary>
    public partial interface IMindAPI
    {
        #region Flux Management

        /// <summary>
        /// Search for fluxes following the given criteria
        /// </summary>
        [Get("/api/v1/flux/search")]
        Task<CustomMindPagedApiResponseBase<FluxSearchResponse>> SearchFluxAsync([Query] FluxSearchRequest request);

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

        ///// <summary>
        ///// Force the fetch of a flux
        ///// </summary>
        //[Get("/api/v1/flux/{id}/force-fetch")]
        //Task<FluxForceFetchResponse> ForceFetchAsync(int id);

        ///// <summary>
        ///// Force the process of a flux
        ///// </summary>
        //[Get("/api/v1/flux/{id}/force-process")]
        //Task<FluxForceProcessResponse> ForceProcessAsync(int id);

        /// <summary>
        /// Force the processing of normalized financial prices for multiple financial IDs
        /// </summary>
        [Post("/api/v1/flux/force-financial-price-processing")]
        Task<ApiResponseBase<ForceFinancialPriceProcessingResponse>> ForceFinancialPriceProcessingAsync([Body] ForceFinancialPriceProcessingRequest request);

        /// <summary>
        /// Force the fetch of a flux in the background without waiting for completion
        /// </summary>
        /// <param name="id">The flux identifier</param>
        /// <returns>Status message indicating that the operation has started</returns>
        [Get("/api/v1/flux/{id}/force-fetch-async")]
        Task<ApiResponseBase<ProcessStartedResponse>> ForceFetchBackgroundAsync(int id);

        /// <summary>
        /// Force the process of a flux in the background without waiting for completion
        /// </summary>
        /// <param name="id">The flux identifier</param>
        /// <returns>Status message indicating that the operation has started</returns>
        [Get("/api/v1/flux/{id}/force-process-async")]
        Task<ApiResponseBase<ProcessStartedResponse>> ForceProcessBackgroundAsync(int id);

        /// <summary>
        /// Force the process of a specific flux fetching content history in the background without waiting for completion
        /// </summary>
        /// <param name="fluxId">The ID of the flux</param>
        /// <param name="fluxFetchingHistoryId">The ID of the flux fetching content history to process</param>
        /// <returns>Status message indicating that the operation has started</returns>
        [Get("/api/v1/flux/fetching-history/{fluxFetchingHistoryId}/force-process-async")]
        Task<ApiResponseBase<ProcessStartedResponse>> ForceProcessElementFetchBackgroundAsync(int fluxId, int fluxFetchingHistoryId);

        /// <summary>
        /// Upload and process a file for a manual flux
        /// </summary>
        /// <param name="fluxId">The flux identifier</param>
        /// <param name="fileName">The name of the uploaded file</param>
        /// <param name="file">The file content stream</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Status message indicating that the operation has started</returns>
        [Multipart]
        [Post("/api/v1/flux/{fluxId}/upload-manual")]
        Task<ApiResponseBase<ProcessStartedResponse>> FetchManualFluxAsync(
            int fluxId,
            string fileName,
            [AliasAs("file")] StreamPart file,
            CancellationToken cancellationToken = default);

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
        Task<CustomMindPagedApiResponseBase<FluxFetchingSearchResponse>> SearchFetchingHistoryAsync([Query] FluxFetchingSearchRequest request);

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
        /// Simulate the processing of a specific flux fetching history
        /// </summary>
        /// <param name="fetchingHistoryId">The ID of the fetching history to simulate processing</param>
        /// <returns>Simulation result with detailed processing information</returns>
        [Get("/api/v1/flux/fetching-history/{fetchingHistoryId}/simulate-process")]
        Task<ApiResponseBase<SimulateProcessElementResponse>> SimulateProcessElementAsync(int fetchingHistoryId);

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
        Task<CustomMindPagedApiResponseBase<FluxProcessingSearchReponse>> SearchProcessingHistoryAsync([Query] FluxProcessingSearchRequest request);

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
        Task<CustomMindPagedApiResponseBase<FluxErrorSearchResponse>> SearchFluxErrorsAsync([Query] FluxErrorSearchRequest request);

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

        #region Rule Errors

        /// <summary>
        /// Search for rule errors following the given criteria
        /// </summary>
        [Get("/api/v1/flux/rule-errors/search")]
        Task<CustomMindPagedApiResponseBase<RuleErrorSearchResponse>> SearchRuleErrorsAsync([Query] RuleErrorSearchRequest request);

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
        [Get("/api/v1/workflow/active")]
        Task<ApiResponseBase<List<ActiveFluxDto>>> GetActiveFluxesAsync();

        /// <summary>
        /// Gets the state of recently completed fluxes
        /// </summary>
        [Get("/api/v1/workflow/completed")]
        Task<ApiResponseBase<List<CompletedFluxDto>>> GetCompletedFluxesAsync([Query] int count = 10);

        /// <summary>
        /// Gets the details of a specific flux workflow
        /// </summary>
        [Get("/api/v1/workflow/{fluxId}")]
        Task<ApiResponseBase<FluxWorkflowDetailsDto>> GetFluxWorkflowDetailsAsync(int fluxId);

        /// <summary>
        /// Gets a workflow by its unique workflow ID
        /// </summary>
        /// <param name="workflowId">The unique identifier of the workflow</param>
        /// <returns>Detailed information about the workflow</returns>
        [Get("/api/v1/workflow/by-id/{workflowId}")]
        Task<ApiResponseBase<FluxWorkflowDetailsDto>> GetWorkflowByIdAsync(Guid workflowId);

        /// <summary>
        /// Gets a global summary of flux workflows
        /// </summary>
        [Get("/api/v1/workflow/summary")]
        Task<ApiResponseBase<WorkflowSummaryDto>> GetFluxWorkflowSummaryAsync();

        /// <summary>
        /// Manually triggers cleanup of historical workflow data
        /// </summary>
        [Post("/api/v1/workflow/cleanup")]
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
        /// Get the size of a raw content file
        /// </summary>
        /// <param name="id">ID of the raw content in MongoDB</param>
        /// <returns>File size information</returns>
        [Get("/api/v1/raw/size/{id}")]
        Task<dynamic> GetFileSize(string id);

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
        /// Search for file uploads following the given criteria
        /// </summary>
        /// <param name="request">Search criteria</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paged list of file uploads</returns>
        [Get("/api/v1/fluxcarac/file-uploads/search")]
        Task<CustomMindPagedApiResponseBase<FileUploadSearchResponse>> SearchFileUploadsAsync(
            [Query] FileUploadSearchRequest request,
            CancellationToken cancellationToken = default);


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
        #endregion

        #region AI Dataset - File Data Mapping

        /// <summary>
        /// Create a new mapping between a file and a financial data point
        /// </summary>
        /// <param name="request">Mapping details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created mapping details</returns>
        [Post("/api/v1/fluxcarac/file-mapping")]
        Task<ApiResponseBase<FileDataMapping>> CreateFileMappingAsync(
            [Body] CreateFileMappingRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a specific file mapping by ID
        /// </summary>
        /// <param name="mappingId">ID of the mapping</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Mapping details</returns>
        [Get("/api/v1/fluxcarac/file-mapping/{mappingId}")]
        Task<ApiResponseBase<FileDataMapping>> GetFileMappingAsync(
            int mappingId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all mappings associated with a specific file upload
        /// </summary>
        /// <param name="fileUploadId">ID of the file upload</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of mappings</returns>
        [Get("/api/v1/fluxcarac/file-mappings/by-file-upload/{fileUploadId}")]
        Task<ApiResponseBase<List<FileDataMapping>>> GetMappingsByFileUploadAsync(
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
        [Post("/api/v1/fluxcarac/property-mapping")]
        Task<ApiResponseBase<PropertyMappingResponse>> CreatePropertyDataTypeAsync(
            [Body] CreatePropertyDataTypeRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Update an existing property data type
        /// </summary>
        [Put("/api/v1/fluxcarac/property-mapping/{id}")]
        Task<ApiResponseBase<PropertyMappingResponse>> UpdatePropertyDataTypeAsync(
            int id,
            [Body] CreatePropertyDataTypeRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a specific property data type
        /// </summary>
        [Get("/api/v1/fluxcarac/property-mapping/{id}")]
        Task<ApiResponseBase<PropertyMappingResponse>> GetPropertyDataTypeAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a property data type
        /// </summary>
        [Delete("/api/v1/fluxcarac/property-mapping/{id}")]
        Task<ApiResponseBase<bool>> DeletePropertyDataTypeAsync(
            int id,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Search property data types
        /// </summary>
        [Get("/api/v1/fluxcarac/property-mapping/search")]
        Task<CustomMindPagedApiResponseBase<PropertyMappingResponse>> SearchPropertyDataTypesAsync(
            [Query] SearchPropertyDataTypeRequest request,
            CancellationToken cancellationToken = default);

        #endregion

        #region Prices

        /// <summary>
        /// Update a price entity
        /// </summary>
        [Post("/api/v1/prices/update")]
        Task<bool> UpdatePriceAsync([Body] UpdatePriceRequest request);

        /// <summary>
        /// Search for prices based on criteria
        /// </summary>
        [Get("/api/v1/prices/search")]
        Task<CustomMindPagedApiResponseBase<SearchPricesResponse>> SearchPricesAsync([Query] SearchPricesRequest request);

        #endregion

        /// <summary>
        /// Search for financial data points based on criteria
        /// </summary>
        /// <param name="request">Search criteria</param>
        /// <returns>Paged list of financial data points</returns>
        [Get("/api/v1/fluxcarac/financial-data-points/search")]
        Task<CustomMindPagedApiResponseBase<FinancialDataPointSearchResponse>> SearchFinancialDataPointsAsync([Query] SearchFinancialDataPointRequest request);

        /// <summary>
        /// Get financial data points by document type with metadata information
        /// </summary>
        /// <param name="documentTypeId">ID of the document type</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Financial data points with metadata for the specified document type</returns>
        [Get("/api/v1/fluxcarac/financial-data-points/by-document-type/{documentTypeId}")]
        Task<ApiResponseBase<GetFinancialDataPointsByDocumentTypeQueryResult>> GetFinancialDataPointsByDocumentTypeAsync(
            int documentTypeId,
            CancellationToken cancellationToken = default);

        #region TradingVenue

        /// <summary>
        /// Get a trading venue by ID or MIC
        /// </summary>
        /// <param name="request">Request with either ID or MIC</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Trading venue details</returns>
        [Get("/api/v1/tradingvenue")]
        Task<ApiResponseBase<TradingVenueResponse>> GetTradingVenueAsync(
            [Query] GetTradingVenueRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Search for trading venues based on criteria
        /// </summary>
        /// <param name="request">Search criteria</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paged list of trading venues</returns>
        [Get("/api/v1/tradingvenue/search")]
        Task<CustomMindPagedApiResponseBase<TradingVenueSearchResponse>> SearchTradingVenuesAsync(
            [Query] SearchTradingVenueRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Edit a trading venue
        /// </summary>
        /// <param name="id">ID of the trading venue to edit</param>
        /// <param name="request">Edit request with updated values</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated trading venue details</returns>
        [Put("/api/v1/tradingvenue/{id}")]
        Task<ApiResponseBase<TradingVenueResponse>> EditTradingVenueAsync(
            int id,
            [Body] EditTradingVenueRequest request,
            CancellationToken cancellationToken = default);

        #endregion

        #region FinancialRules

        /// <summary>
        /// Search for financial rules based on a specific data point or get all rules
        /// </summary>
        /// <param name="dataPoint">Optional financial technical data point to filter rules</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Financial rules with markdown documentation</returns>
        [Get("/api/v1/fluxcarac/financial-rules")]
        Task<ApiResponseBase<SearchFinancialRuleQueryResult>> SearchFinancialRulesAsync(
            [Query] FinancialTechnicalDataPoint? dataPoint = null,
            CancellationToken cancellationToken = default);

        #endregion

        #region Languages
        /// <summary>
        /// Get language by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Get("/api/v1/language/{id}")]
        Task<GetLanguageResponse> GetLanguageAsync(int id);

        /// <summary>
        /// List all active languages
        /// </summary>
        /// <returns></returns>
        [Get("/api/v1/language/list")]
        Task<ListLanguageResponse> ListLanguagesAsync();

        /// <summary>
        /// Create a language
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Post("/api/v1/language")]
        Task<GetLanguageResponse> CreateLanguageAsync([Body] SaveLanguageRequest request);


        /// <summary>
        /// Update a language
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Put("/api/v1/language/{id}")]
        Task<GetLanguageResponse> UpdateLanguageAsync(int id, [Body] SaveLanguageRequest request);

        /// <summary>
        /// Delete a language by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Delete("/api/v1/language/{id}")]
        Task<DeletedResponse> DeleteLanguageAsync(int id);

        #endregion

        #region DocumentTypes
        /// <summary>
        /// Get language by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Get("/api/v1/document-type/{id}")]
        Task<GetDocumentTypeResponse> GetDocumentTypeAsync(int id);

        /// <summary>
        /// List all active languages
        /// </summary>
        /// <returns></returns>
        [Get("/api/v1/document-type/search")]
        Task<ListDocumentTypesResponse> SearchDocumentTypesAsync(
            [Query] SearchDocumentTypeRequest request);

        /// <summary>
        /// Create a language
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Post("/api/v1/document-type")]
        Task<GetDocumentTypeResponse> CreateDocumentTypeAsync([Body] SaveDocumentTypeRequest request);


        /// <summary>
        /// Update a language
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Put("/api/v1/document-type/{id}")]
        Task<GetDocumentTypeResponse> UpdateDocumentTypeAsync(int id, [Body] SaveDocumentTypeRequest request);

        /// <summary>
        /// Delete a language by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Delete("/api/v1/document-type/{id}")]
        Task<DeletedResponse> DeleteDocumentTypeAsync(int id);

        #endregion

        #region AI Dataset - Element Metadatas

        /// <summary>
        /// Search financial data point element metadatas
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="documentTypeId"></param>
        /// <param name="languageCode"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Get("/api/v1/fluxcarac/financial-data-point-element/metadata/search")]
        Task<ListMetadatasResponse> SearchElementMetadataAsync(
            [Query] int elementId,
            [Query] int? documentTypeId,
            [Query] string? languageCode,
            [Query] FinancialDataPointElementMetadataKey? key);

        /// <summary>
        /// Create financial data point element metadata
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Post("/api/v1/fluxcarac/financial-data-point-element/metadata")]
        Task<GetMetadataResponse> CreateElementMetadataAsync([Body] SaveMetadataRequest request);

        /// <summary>
        /// Update financial data point element metadata
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Put("/api/v1/fluxcarac/financial-data-point-element/metadata")]
        Task<GetMetadataResponse> UpdateElementMetadataAsync([Body] SaveMetadataRequest request);

        /// <summary>
        /// Delete financial data point element metadata
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Delete("/api/v1/fluxcarac/financial-data-point-element/metadata")]
        Task<DeletedResponse> DeleteElementMetadataAsync([Body] DeleteMetadataRequest request);

        #endregion

        #region "Mails"
        /// <summary>
        /// jhj
        /// </summary>
        /// <returns></returns>
        [Post("/api/v1/flux/fetch-emails")]
        Task<ApiResponseBase<FetchEmailMetadataCommandResult>> FetchEmailMetadataAsync();

        #endregion
    }
}
