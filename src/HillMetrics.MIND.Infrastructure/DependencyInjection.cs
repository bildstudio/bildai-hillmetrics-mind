using HillMetrics.Core.AI.Configs;
using HillMetrics.Core.AI.Contracts;
using HillMetrics.Core.API.Contracts;
using HillMetrics.Core.Authentication.Contracts;
using HillMetrics.MIND.Infrastructure.AI;
using HillMetrics.MIND.Infrastructure.Authentication;
using HillMetrics.Normalized.Domain.Contracts.Repository;
using HillMetrics.Normalized.Infrastructure.Database.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HillMetrics.MIND.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMindAuthenticationServices(this IServiceCollection services)
        {
            services.TryAddSingleton<IAuthenticationService, AzureAdAuthenticationService>();
            services.TryAddSingleton<IRedirectUrlValidator, RedirectUrlValidator>();

            return services;
        }


        public static IServiceCollection AddMindAiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddTransient<IAiModelPromptRepository, AiModelPromptRepository>();
            services.TryAddTransient<ILlmServiceFactory, LlmServiceFactory>();

            //AiLlmConfig config = new AiLlmConfig();
            //configuration.GetSection("AI:Models").Bind(config);

            //configuration.GetSection("AI").Bind(config);

            services.Configure<AiLlmConfig>(configuration.GetSection("AI"));

            return services;
        }
    }
}
