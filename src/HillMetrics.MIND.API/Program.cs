
//using Asp.Versioning;
using Asp.Versioning;
using HillMetrics.Core;
using HillMetrics.Core.API;
using HillMetrics.Core.API.Extensions;
using HillMetrics.Core.Authentication;
using HillMetrics.Core.Monitoring.Logging;
using HillMetrics.MIND.Domain;
using HillMetrics.MIND.Infrastructure;
using HillMetrics.Normalized.Infrastructure.Database.Database;
using HillMetrics.Orchestrator.ServicesNames;


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
        builder.AddHillMetricsCoreExtension();
        
        //MIND services
        builder.AddHillMetricsMINDServices();

        var logger = builder.InitAndAddHillMetricsLogger("HillMetrics.MIND.API");

        //ADD Aspire discover/open telemetry
        builder.AddServiceDefaults();

        //Set default binding and responses for datetimes to be UTC
        builder.Services.AddControllers(options =>
        {
            options.ModelBinderProviders.Insert(0, new DateTimeUTCModelBinderProvider());
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonDateTimeUTCConverter());
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHealthChecks();

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

        app.MigrateNormalizedDatabase(logger);

        app.Run();
    }
}
