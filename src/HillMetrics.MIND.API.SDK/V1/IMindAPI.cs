using HillMetrics.MIND.API.Contracts.Requests.Flux;
using HillMetrics.MIND.API.Contracts.Requests.Source;
using HillMetrics.MIND.API.Contracts.Responses;
using HillMetrics.MIND.API.Contracts.Responses.Flux;
using HillMetrics.MIND.API.Contracts.Responses.Source;
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
        Task<FluxResponse> GetFluxAsync(int id);

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
        Task<PagedApiResponseBase<FluxErrorSearchDto>> SearchFluxErrorsAsync([Query] FluxErrorSearchRequest request);

        /// <summary>
        /// Get the details of a specific error
        /// </summary>
        [Get("/api/v1/flux/errors/{errorId}")]
        Task<ApiResponseBase<FluxErrorResponse>> GetErrorAsync(int errorId);

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
    }
}
