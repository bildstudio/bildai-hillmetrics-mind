using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace HillMetrics.MIND.FrontApp.Services.Base
{
    public abstract class BaseService
    {
        protected readonly HttpClient HttpClient;
        protected readonly JsonSerializerOptions DefaultJsonOptions;

        protected BaseService(HttpClient httpClient)
        {
            HttpClient = httpClient;
            DefaultJsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        protected async Task<T?> GetAsync<T>(string uri) where T : class
        {
            try
            {
                return await HttpClient.GetFromJsonAsync<T>(uri, DefaultJsonOptions);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"An error occurred while sending the request: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"An error occurred while deserializing the response: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                return null;
            }
        }

        protected async Task<TResponse?> PostAsync<TResponse>(string uri) where TResponse : class
        {
            try
            {
                var response = await HttpClient.PostAsync(uri, null);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TResponse>(DefaultJsonOptions);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"An error occurred while sending the request: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"An error occurred while deserializing the response: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                return null;
            }
        }

        protected async Task<TResponse?> PostAsync<TResponse, TRequest>(string uri, TRequest request) 
            where TResponse : class
            where TRequest : class
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(request, DefaultJsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                var response = await HttpClient.PostAsync(uri, content);
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadFromJsonAsync<TResponse>(DefaultJsonOptions);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"An error occurred while sending the request: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"An error occurred while deserializing the response: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                return null;
            }
        }

        protected async Task<TResponse?> PutAsync<TResponse, TRequest>(string uri, TRequest request) 
            where TResponse : class
            where TRequest : class
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(request, DefaultJsonOptions);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                var response = await HttpClient.PutAsync(uri, content);
                response.EnsureSuccessStatusCode();
                
                return await response.Content.ReadFromJsonAsync<TResponse>(DefaultJsonOptions);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"An error occurred while sending the request: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"An error occurred while deserializing the response: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                return null;
            }
        }

        protected async Task<bool> DeleteAsync(string uri)
        {
            try
            {
                var response = await HttpClient.DeleteAsync(uri);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during delete operation: {ex.Message}");
                return false;
            }
        }
    }
} 