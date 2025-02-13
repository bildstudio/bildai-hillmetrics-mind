using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using HillMetrics.Core.Authentication.Keycloak;
using HillMetrics.Core.Authentication.Objects;
using HillMetrics.MIND.Infrastructure;

namespace HillMetrics.MIND.API.Extensions
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;
        private readonly KeycloakConfigMind _keycloakConfig;

        public ConfigureSwaggerOptions(
            IApiVersionDescriptionProvider provider, 
            IOptions<KeycloakConfigMind> keycloakOptions
            )
        {
            _provider = provider;
            _keycloakConfig = keycloakOptions.Value;
        }

        public void Configure(SwaggerGenOptions options)
        {
            options.UseInlineDefinitionsForEnums();

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }

            options.AddKeycloakBearerAuthentication(_keycloakConfig, AuthenticationType.AuthorizationCode);
        }

        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo
            {
                Title = "Hill Metrics BFF API",
                Version = description.ApiVersion.ToString(),
                Description = "API for usage only by HillMetrics Frontend application"
            };
            return info;
        }
    }
}
