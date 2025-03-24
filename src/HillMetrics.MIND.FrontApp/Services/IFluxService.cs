using HillMetrics.MIND.API.Contracts.Requests.Flux;
using HillMetrics.MIND.API.Contracts.Responses;
using HillMetrics.MIND.API.Contracts.Responses.Flux;

namespace HillMetrics.MIND.FrontApp.Services
{
    public interface IFluxService
    {
        Task<PagedApiResponseBase<FluxSearchResponse>?> SearchAsync(FluxSearchRequest request);
        Task<FluxResponse?> GetAsync(int id);
        Task<bool> CreateFluxAsync(FluxRequest request);
        Task<bool> UpdateFluxAsync(int fluxId, FluxRequest request);
        Task<bool> DeleteFluxAsync(int fluxId);
        Task<FluxForceFetchResponse?> ForceFetchAsync(int id);
        Task<FluxForceProcessResponse?> ForceProcessAsync(int id);
        Task<PagedApiResponseBase<FluxProcessingSearchReponse>?> SearchProcessingHistoryAsync(FluxProcessingSearchRequest request);
        Task<PagedApiResponseBase<FluxErrorSearchResponse>?> SearchErrorsAsync(FluxErrorSearchRequest request);
        Task<FluxFetchingResponse?> GetFetchingHistoryAsync(int id);
        Task<FluxProcessingResponse?> GetProcessingHistoryAsync(int id);
        Task<PagedApiResponseBase<FluxFetchingSearchResponse>?> SearchFetchingHistoryAsync(FluxFetchingSearchRequest request);
        Task<ApiResponseBase<FluxErrorResponse>> GetErrorAsync(int errorId);
    }
} 