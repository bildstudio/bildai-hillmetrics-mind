using HillMetrics.MIND.API.Contracts.Converter;
using HillMetrics.MIND.API.SDK.V1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Refit;
using System.Text.Json;

namespace HillMetrics.MIND.API.SDK
{
    /// <summary>
    /// Provides extension methods to register MIND API SDK services in an ASP.NET Core service container
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds the MIND API client with a custom HTTP handler, consumer identifier, and configurable timeout
        /// </summary>
        /// <typeparam name="THttpMessageHandler">The type of HTTP handler to use</typeparam>
        /// <param name="services">The service collection</param>
        /// <param name="baseAddress">The base address of the MIND API</param>
        /// <param name="consumer">The API consumer identifier</param>
        /// <param name="httpClientTimeout">The HTTP client timeout (default: 30 seconds)</param>
        /// <returns>The HTTP client builder for additional configuration</returns>
        public static IHttpClientBuilder AddMindApiSDK<THttpMessageHandler>(
           this IServiceCollection services,
           string baseAddress,
           string consumer,
           TimeSpan? httpClientTimeout = null
           )
            where THttpMessageHandler : DelegatingHandler
        {
            httpClientTimeout ??= TimeSpan.FromSeconds(30);

            IHttpClientBuilder httpClientBuilder = services.AddRefitClient<IMindAPI>(settings: new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                }),
            })
                .ConfigureHttpClient(s =>
                {
                    s.BaseAddress = new(baseAddress);
                    s.DefaultRequestHeaders.Add("x-caller", consumer);
                    s.Timeout = httpClientTimeout.Value;
                })
                .AddHttpMessageHandler<THttpMessageHandler>();

            services.TryAddTransient<THttpMessageHandler>();

            return httpClientBuilder;
        }

        /// <summary>
        /// Adds the MIND API client with an existing HTTP client
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="httpClient">The HTTP client to use</param>
        /// <returns>The updated service collection</returns>
        public static IServiceCollection AddMindApiSDK(
            this IServiceCollection services,
            HttpClient httpClient
            )
        {
            IMindAPI api = RestService.For<IMindAPI>(httpClient);
            services.AddSingleton<IMindAPI>(api);
            return services;
        }

        /// <summary>
        /// Adds the MIND API client with only a base address
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="baseAddress">The base address of the MIND API</param>
        /// <returns>The updated service collection</returns>
        public static IServiceCollection AddMindApiSDK(
            this IServiceCollection services,
            string baseAddress
            )
        {
            var refitSettings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
                {
                    Converters = { new FluxMetadataDtoJsonConverter() },
                }),
            };

            services.AddRefitClient<IMindAPI>()
                .ConfigureHttpClient(s =>
                {
                    s.BaseAddress = new(baseAddress);
                    s.Timeout = TimeSpan.FromMinutes(5);
                });

            return services;
        }
    }
}