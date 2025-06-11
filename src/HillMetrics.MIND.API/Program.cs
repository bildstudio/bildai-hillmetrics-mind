using Asp.Versioning;
using HillMetrics.Audit.Infrastructure.Database;
using HillMetrics.Core.API;
using HillMetrics.Core.API.Convention;
using HillMetrics.Core.API.Extensions;
using HillMetrics.Core.Authentication;
using HillMetrics.Core.Flux.Extension;
using HillMetrics.MIND.API.Extensions;
using HillMetrics.MIND.API.Mappers;
using HillMetrics.MIND.Infrastructure;
using HillMetrics.Normalized.Domain.Extensions;
using HillMetrics.Normalized.Domain.UseCase.Market.Price;
using HillMetrics.Normalized.Infrastructure.Database.Database;
using HillMetrics.Orchestrator.ServicesNames;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;
using HillMetrics.Python.API.SDK;
using HillMetrics.Core.Mediator.Extensions;
using HillMetrics.Core.Monitoring.Audits;
using HillMetrics.MIND.API.Contracts.Converter;
using HillMetrics.MIND.API.Consumers;
using HillMetrics.Core.Messaging.Services;
using HillMetrics.Core.Converters;
using HillMetrics.MIND.Infrastructure.Database.Extensions;
using HillMetrics.MIND.Infrastructure.Database.Database;
using HillMetrics.MIND.Domain;
using HillMetrics.Core.Rules;
using HillMetrics.Business.API.SDK;
using HillMetrics.Core.Authentication.Keycloak.HttpHandlers;
using HillMetrics.Normalized.Domain.Rules;

namespace HillMetrics.MIND.API;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //Core services
        var logger = builder.ConfigureCommonFluxService(Services.MindAPI, typeof(Program), typeof(PriceBondHandler), s => {
            s.AddConsumer<SignalrSubscribeEventConsumerTest>();
        });

        logger.LogInformation("Starting {applicationName}...", Services.MindAPI);

        builder.Services.AddDomainServices();
        builder.Services.AddFluxWorkflowTracker();
        builder.Services.AddFinancialRules([typeof(PriceParsingExtendedRule).Assembly]);

        builder.Services.AddAutoMapper(typeof(FluxMappingProfile));
        builder.Services.AddAutoMapper(typeof(GicsMappingProfile));
        builder.Services.AddAutoMapper(typeof(AiDatasetMappingProfile));
        builder.Services.AddAutoMapper(typeof(FinancialMappingProfile));
        builder.Services.AddAutoMapper(typeof(LlmMappingProfile));
        builder.Services.AddAutoMapper(typeof(FinancialDataPointMappingProfile));

        //add cors
        builder.AddHillMetricsCorsSettings();

        //Set default binding and responses for datetimes to be UTC
        builder.Services.AddControllers(options =>
        {
            options.ModelBinderProviders.Insert(0, new DateTimeUTCModelBinderProvider());
            options.Conventions.Add(new LowercaseControllerRouteConvention());
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonDateTimeUTCConverter());
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new FluxMetadataDtoJsonConverter());
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        });

        builder.Services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        });


        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        //configure swagger options to have authentication option in swagger UI, xml comments, etc
        builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        builder.Services.AddHillMetricsApiVersioning(new ApiVersion(1), [new UrlSegmentApiVersionReader()]);

        builder.AddHillMetricsRateLimiters();

        //validation for JWT token authentication
        builder.AddKeycloakBearerAuthenticationValidator<KeycloakConfigMind>(serviceName: Services.Keycloak);

        builder.Services.AddMindAuthenticationServices(builder.Configuration);

        builder.Services.AddMindAiServices(builder.Configuration);

        builder.Services.AddPythonApiServices(builder.Configuration, "mind-api", TimeSpan.FromMinutes(2));

        // Add Business API SDK with authentication
        var businessApi = builder.Configuration.GetValue<string>("Services:BusinessApi", "https+http://BusinessAPI");
        builder.Services.AddBusinessApiSDK<PrivateAuthenticationHttpHandler>(businessApi, "MindApi", httpClientTimeout: TimeSpan.FromMinutes(10));

        builder.Services.AddHillMetricsAuditedRequestsPreProcessors();
        builder.Services.AddHillMetricsAuditServices(Core.AuditApplicationName.HillMetrics_Mind);

        builder.Services.AddHillMetricsSignalRServices();
        builder.AddHillMetricsMINDServices();
        builder.AddMindAppDatabaseProvider(builder.Configuration.GetConnectionString(Services.MindApplicationDb));

        var app = builder.Build();

        app.UseHillMetricsCorsSettings();

        app.UseHillMetricsRateLimiters();

        app.UseHillMetricsApiExceptionHandlerMiddleware();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var descriptions = app.DescribeApiVersions();
            foreach (var description in descriptions)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            }

            KeycloakConfigMind config = app.Services.GetRequiredService<IOptions<KeycloakConfigMind>>().Value;
            if (config.Azure != null)
            {
                options.OAuthClientId(config.Azure.ClientId);
                options.OAuthAppName("Swagger UI - Azure AD authentication");
                options.OAuthUsePkce();
                options.OAuthAdditionalQueryStringParams(new Dictionary<string, string>
                {
                    { "kc_idp_hint", config.Azure.ProviderAlias }
                });
            }
        });

        //app.Services.GetKeyedServices<IChatClient>(KeyedService.AnyKey);

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MigrateAuditDatabase(logger);
        app.MigrateNormalizedDatabase(logger);
        app.MigrateMindAppDatabase(logger);
        app.MigrateMindAppClientsDatabasesAsync(logger).GetAwaiter().GetResult();

        app.Run();
    }
}