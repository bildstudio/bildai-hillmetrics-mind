using HillMetrics.Core.API.Contracts;
using HillMetrics.Core.Authentication.Contracts;
using HillMetrics.MIND.Infrastructure.Authentication;
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
    }
}
