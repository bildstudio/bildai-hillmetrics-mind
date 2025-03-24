
//using Asp.Versioning;
using Asp.Versioning;
using HillMetrics.Audit.Infrastructure.Database;
using HillMetrics.Core.API;
using HillMetrics.Core.API.Convention;
using HillMetrics.Core.API.Extensions;
using HillMetrics.Core.Authentication;
using HillMetrics.Core.Flux.Extension;
using HillMetrics.MIND.API.Converter;
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
using HillMetrics.Core.Contracts;
using HillMetrics.Core.API.Services;
using MediatR.Pipeline;
using HillMetrics.Core.Mediator.Processors;
using MediatR;
using HillMetrics.Core.Mediator.Extensions;
using HillMetrics.Core.Monitoring.Audits;
using HillMetrics.Core.Monitoring;
using HillMetrics.Core.Messaging.Notification.Services;
using HillMetrics.MIND.API.Consumers;


namespace HillMetrics.MIND.API;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //IWebHostEnvironment  environment = builder.Environment;
        //builder.Configuration
        //    .SetBasePath(environment.ContentRootPath)
        //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        //    .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        //    .AddEnvironmentVariables();

        // Add services to the container.

        //Core services
        var logger = builder.ConfigureCommonFluxService(Services.MindAPI, typeof(Program), typeof(PriceBondHandler), s => {
            s.AddConsumer<SignalrSubscribeEventConsumerTest>();
        });

        logger.LogInformation("Starting {applicationName}...", Services.MindAPI);

        builder.Services.AddDomainServices();
        builder.Services.AddFluxWorkflowTracker();

        builder.Services.AddAutoMapper(typeof(FluxMappingProfile));
        builder.Services.AddAutoMapper(typeof(GicsMappingProfile));

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

        builder.Services.AddMindAuthenticationServices();

        builder.Services.AddMindAiServices(builder.Configuration);

        builder.Services.AddPythonApiServices(builder.Configuration, "mind-api", TimeSpan.FromMinutes(2));


        builder.Services.AddHillMetricsAuditedRequestsPreProcessors();
        builder.Services.AddHillMetricsAuditServices(Core.AuditApplicationName.HillMetrics_Mind);

        builder.Services.AddHillMetricsSignalRServices();

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

        app.Run();
    }
}
