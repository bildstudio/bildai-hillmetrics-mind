//using HillMetrics.MIND.API.Contracts.Requests.Flux;
//using HillMetrics.MIND.API.Contracts.Responses;
//using HillMetrics.MIND.API.Contracts.Responses.Flux;
//using HillMetrics.Normalized.Domain.Contracts.Providing.Flux.Cqrs.Collect;
//using HillMetrics.Normalized.Domain.Contracts.Providing.Flux.Cqrs.Process;

//namespace HillMetrics.MIND.Web.Services
//{
//    public class FluxService : IFluxService
//    {
//        private readonly HttpClient _httpClient;
//        private const string ApiEndpoint = "api/v1/Flux";

//        public FluxService(HttpClient httpClient)
//        {
//            _httpClient = httpClient;
//        }

//        public async Task<PagedApiResponseBase<FluxSearchResponse>> SearchAsync(FluxSearchRequest request)
//        {
//            var response = await _httpClient.GetFromJsonAsync<PagedApiResponseBase<FluxSearchResponse>>($"{ApiEndpoint}/search?{GetQueryString(request)}");
//            return response!;
//        }

//        public async Task<FluxResponse> GetAsync(int id)
//        {
//            var response = await _httpClient.GetFromJsonAsync<FluxResponse>($"{ApiEndpoint}/{id}");
//            return response!;
//        }

//        public async Task<bool> CreateFluxAsync(FluxRequest request)
//        {
//            var response = await _httpClient.PostAsJsonAsync(ApiEndpoint, request);
//            response.EnsureSuccessStatusCode();
//            return await response.Content.ReadFromJsonAsync<bool>();
//        }

//        public async Task<bool> UpdateFluxAsync(int fluxId, FluxRequest request)
//        {
//            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoint}?fluxId={fluxId}", request);
//            response.EnsureSuccessStatusCode();
//            return await response.Content.ReadFromJsonAsync<bool>();
//        }

//        public async Task<bool> DeleteFluxAsync(int fluxId)
//        {
//            var response = await _httpClient.DeleteAsync($"{ApiEndpoint}?fluxId={fluxId}");
//            response.EnsureSuccessStatusCode();
//            return await response.Content.ReadFromJsonAsync<bool>();
//        }

//        public async Task<FetchFluxCommandResult> ForceFetchAsync(int id)
//        {
//            var response = await _httpClient.GetFromJsonAsync<FetchFluxCommandResult>($"{ApiEndpoint}/{id}/force-fetch");
//            return response!;
//        }

//        public async Task<ProcessFluxCommandResult> ForceProcessAsync(int id)
//        {
//            var response = await _httpClient.GetFromJsonAsync<ProcessFluxCommandResult>($"{ApiEndpoint}/{id}/force-process");
//            return response!;
//        }

//        private string GetQueryString(object obj)
//        {
//            var properties = obj.GetType().GetProperties()
//                .Where(p => p.GetValue(obj, null) != null)
//                .Select(p => $"{p.Name}={Uri.EscapeDataString(p.GetValue(obj, null)!.ToString()!)}");
            
//            return string.Join("&", properties);
//        }
//    }
//} 