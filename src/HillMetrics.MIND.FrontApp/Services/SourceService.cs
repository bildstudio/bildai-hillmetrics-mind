using System.Net.Http.Json;
using HillMetrics.MIND.API.Contracts.Requests.Source;
using HillMetrics.MIND.API.Contracts.Responses.Flux;
using HillMetrics.MIND.API.Contracts.Responses.Source;

namespace HillMetrics.MIND.FrontApp.Services
{
    public class SourceService : ISourceService
    {
        private readonly HttpClient _httpClient;
        private const string ApiEndpoint = "api/v1/Source";

        public SourceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SourceSearchResponse> SearchAsync(SourceSearchRequest request)
        {
            var queryString = GetQueryString(request);
            var response = await _httpClient.GetFromJsonAsync<SourceSearchResponse>($"{ApiEndpoint}/search?{queryString}");
            return response!;
        }

        public async Task<SourceResponse> CreateSourceAsync(SourceCreateRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiEndpoint, request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SourceResponse>()!;
        }

        public async Task<SourceResponse> EditSourceAsync(int sourceId, SourceCreateRequest request)
        {
            var response = await _httpClient.PutAsJsonAsync($"{ApiEndpoint}?sourceId={sourceId}", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SourceResponse>()!;
        }

        private string GetQueryString(object obj)
        {
            var properties = obj.GetType().GetProperties()
                .Where(p => p.GetValue(obj, null) != null)
                .Select(p => $"{p.Name}={Uri.EscapeDataString(p.GetValue(obj, null)!.ToString()!)}");
            
            return string.Join("&", properties);
        }
    }
} 