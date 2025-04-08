using HillMetrics.Core.AI.Configs;
using HillMetrics.Core.AI.Contracts;
using HillMetrics.Core.API.Configs;
using HillMetrics.Core.API.Contracts;
using HillMetrics.Core.API.Services;
using HillMetrics.Core.Authentication.Contracts;
using HillMetrics.MIND.Infrastructure.AI;
using HillMetrics.MIND.Infrastructure.Authentication;
using HillMetrics.Normalized.Domain.Contracts.Repository;
using HillMetrics.Normalized.Infrastructure.Database.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace HillMetrics.MIND.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMindAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddSingleton<IAuthenticationService, AzureAdAuthenticationService>();
            services.TryAddSingleton<ITokenExchangeService, TokenExchangeService>();

            //inject redis multiplexer as we use it in TokenExchangeService
            string? redisConnectionString = configuration.GetConnectionString("Redis");
            if (string.IsNullOrEmpty(redisConnectionString))
            {
                string errorMsg = "'Redis' connectionString is missing in ConnectionStrings section";
                throw new Exception(errorMsg);
            }

            services.AddSingleton<IConnectionMultiplexer>(s => ConnectionMultiplexer.Connect(redisConnectionString));

            services.TryAddSingleton<IRedirectUrlValidator, RedirectUrlValidator>();

            IConfigurationSection corsSection = configuration.GetSection("CookieOptions");
            services.Configure<CookieConfig>(corsSection);
            services.TryAddSingleton<ICookieService, CookieService>();

            return services;
        }


        public static IServiceCollection AddMindAiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddTransient<IAiModelPromptRepository, AiModelPromptRepository>();
            services.TryAddTransient<ILlmServiceFactory, LlmServiceFactory>();

            services.Configure<AiLlmConfig>(configuration.GetSection("AI"));

            return services;
        }
    }
}
