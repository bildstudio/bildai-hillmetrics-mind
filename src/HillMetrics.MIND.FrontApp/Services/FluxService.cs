using System.Net.Http.Json;
using HillMetrics.MIND.API.Contracts.Requests.Flux;
using HillMetrics.MIND.API.Contracts.Responses;
using HillMetrics.MIND.API.Contracts.Responses.Flux;
using HillMetrics.MIND.FrontApp.Services.Base;

namespace HillMetrics.MIND.FrontApp.Services
{
    public class FluxService : BaseService, IFluxService
    {
        public FluxService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<PagedApiResponseBase<FluxSearchResponse>?> SearchAsync(FluxSearchRequest request)
        {
            return await PostAsync<PagedApiResponseBase<FluxSearchResponse>, FluxSearchRequest>("api/flux/search", request);
        }

        public async Task<FluxResponse?> GetAsync(int id)
        {
            return await GetAsync<FluxResponse>($"api/flux/{id}");
        }

        public async Task<bool> CreateFluxAsync(FluxRequest request)
        {
            var response = await HttpClient.PostAsJsonAsync("api/flux", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateFluxAsync(int fluxId, FluxRequest request)
        {
            var response = await HttpClient.PutAsJsonAsync($"api/flux/{fluxId}", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteFluxAsync(int fluxId)
        {
            var response = await HttpClient.DeleteAsync($"api/flux/{fluxId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<FluxForceFetchResponse?> ForceFetchAsync(int id)
        {
            return await PostAsync<FluxForceFetchResponse>($"api/flux/{id}/force-fetch");
        }

        public async Task<FluxForceProcessResponse?> ForceProcessAsync(int id)
        {
            return await PostAsync<FluxForceProcessResponse>($"api/flux/{id}/force-process");
        }

        public async Task<PagedApiResponseBase<FluxProcessingSearchReponse>?> SearchProcessingHistoryAsync(FluxProcessingSearchRequest request)
        {
            return await PostAsync<PagedApiResponseBase<FluxProcessingSearchReponse>, FluxProcessingSearchRequest>("api/flux/processing/search", request);
        }

        public async Task<PagedApiResponseBase<FluxErrorSearchDto>?> SearchErrorsAsync(FluxErrorSearchRequest request)
        {
            return await PostAsync<PagedApiResponseBase<FluxErrorSearchDto>, FluxErrorSearchRequest>("api/flux/errors/search", request);
        }

        public async Task<FluxFetchingResponse?> GetFetchingHistoryAsync(int id)
        {
            return await GetAsync<FluxFetchingResponse>($"api/flux/fetching/{id}");
        }

        public async Task<FluxProcessingResponse?> GetProcessingHistoryAsync(int id)
        {
            return await GetAsync<FluxProcessingResponse>($"api/flux/processing/{id}");
        }

        public async Task<PagedApiResponseBase<FluxFetchingSearchResponse>?> SearchFetchingHistoryAsync(FluxFetchingSearchRequest request)
        {
            return await PostAsync<PagedApiResponseBase<FluxFetchingSearchResponse>, FluxFetchingSearchRequest>("api/flux/fetching/search", request);
        }

        public async Task<PagedApiResponseBase<FluxErrorSearchDto>> SearchErrorAsync(FluxErrorSearchRequest request)
        {
            return await PostAsync<PagedApiResponseBase<FluxErrorSearchDto>, FluxErrorSearchRequest>("api/flux/errors/search", request);
        }

        public async Task<ApiResponseBase<FluxErrorResponse>> GetErrorAsync(int errorId)
        {
            return await GetAsync<ApiResponseBase<FluxErrorResponse>>($"api/flux/errors/{errorId}");
        }
    }
} 