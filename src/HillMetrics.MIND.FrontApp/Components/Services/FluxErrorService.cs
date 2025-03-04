//using HillMetrics.MIND.API.Contracts.Requests.Flux;
//using HillMetrics.MIND.API.Contracts.Responses.Flux;
//using HillMetrics.MIND.API.Contracts.Responses;

//namespace HillMetrics.MIND.Web.Services
//{
//    public class FluxErrorService : IFluxErrorService
//    {
//        private readonly HttpClient _httpClient;
//        private const string ApiEndpoint = "api/v1/Flux/errors";

//        public FluxErrorService(HttpClient httpClient)
//        {
//            _httpClient = httpClient;
//        }

//        public async Task<PagedApiResponseBase<FluxErrorSearchDto>> SearchAsync(FluxErrorSearchRequest request)
//        {
//            var response = await _httpClient.GetFromJsonAsync<PagedApiResponseBase<FluxErrorSearchDto>>($"{ApiEndpoint}/search?{GetQueryString(request)}");
//            return response!;
//        }

//        public async Task<ApiResponseBase<FluxErrorResponse>> GetAsync(int errorId)
//        {
//            var response = await _httpClient.GetFromJsonAsync<ApiResponseBase<FluxErrorResponse>>($"{ApiEndpoint}/{errorId}");
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