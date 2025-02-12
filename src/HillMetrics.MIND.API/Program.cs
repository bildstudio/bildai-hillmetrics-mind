
//using Asp.Versioning;
using Asp.Versioning;
using HillMetrics.Audit.Infrastructure.Database;
using HillMetrics.Audit.Infrastructure.Database.Extensions;
using HillMetrics.Core;
using HillMetrics.Core.API;
using HillMetrics.Core.API.Convention;
using HillMetrics.Core.API.Extensions;
using HillMetrics.Core.Authentication;
using HillMetrics.Core.Common.Email;
using HillMetrics.Core.Flux.Extension;
using HillMetrics.Core.Mediator.Extensions;
using HillMetrics.Core.Monitoring.Logging;
using HillMetrics.Core.Storage.Extensions;
using HillMetrics.MIND.API.Converter;
using HillMetrics.MIND.API.Mappers;
using HillMetrics.MIND.Domain;
using HillMetrics.MIND.Infrastructure;
using HillMetrics.Normalized.Domain.Contracts.Providing.Flux.Cqrs.Get;
using HillMetrics.Normalized.Domain.Extensions;
using HillMetrics.Normalized.Domain.UseCase.Providing.Flux;
using HillMetrics.Normalized.Infrastructure.Database.Database;
using HillMetrics.Normalized.Infrastructure.Database.Extensions;
using HillMetrics.Orchestrator.ServicesNames;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using static HillMetrics.Normalized.Domain.UseCase.Market.MarketNavValidator;


namespace HillMetrics.MIND.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        IWebHostEnvironment  environment = builder.Environment;
        builder.Configuration
            .SetBasePath(environment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        // Add services to the container.

        //Core services
        var logger = builder.ConfigureCommonFluxService("HillMetrics.MIND.API", typeof(SearchFluxHandler), typeof(SearchFluxQuery));
        builder.Services.AddDomainServices();
        builder.Services.AddAutoMapper(typeof(FluxMappingProfile));

        //var logger = builder.InitAndAddHillMetricsLogger("HillMetrics.MIND.API");

        //ADD Aspire discover/open telemetry
        //builder.AddServiceDefaults();

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
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            c.UseInlineDefinitionsForEnums();

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        builder.Services.AddHillMetricsApiVersioning(new ApiVersion(1), [new UrlSegmentApiVersionReader()]);

        builder.AddHillMetricsRateLimiters();

        //validation for JWT token authentication
        builder.AddKeycloakBearerAuthenticationValidator<KeycloakConfigMind>(serviceName: Services.Keycloak);

        builder.Services.AddMindAuthenticationServices();

        var app = builder.Build();

        app.UseHillMetricsRateLimiters();

        app.UseHillMetricsApiExceptionHandlerMiddleware();

        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        //{
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var descriptions = app.DescribeApiVersions();
            foreach (var description in descriptions)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            }
        });

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MigrateAuditDatabase(logger);
        app.MigrateNormalizedDatabase(logger);

        app.Run();
    }
}
