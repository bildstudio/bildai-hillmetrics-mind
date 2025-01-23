
//using Asp.Versioning;
using Asp.Versioning;
using HillMetrics.Core;
using HillMetrics.Core.API;
using HillMetrics.Core.API.Extensions;
using HillMetrics.Core.Monitoring.Logging;
using HillMetrics.MIND.Domain;
using HillMetrics.Normalized.Infrastructure.Database.Database;


namespace HillMetrics.MIND.API;

public class Program
{
    private static IWebHostEnvironment _environment;

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        _environment = builder.Environment;
        builder.Configuration
            .SetBasePath(_environment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{_environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        // Add services to the container.
        //Core services
        builder.Services.AddHillMetricsCoreExtension();
        
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

        app.UseAuthorization();

        app.MapControllers();

        app.MigrateNormalizedDatabase(logger);

        app.Run();
    }
}
