using HillMetrics.MIND.FrontApp.Components;
using MudBlazor.Services;
using MudBlazor;
using HillMetrics.MIND.API.SDK;
using HillMetrics.Core;
using HillMetrics.Core.Monitoring.Logging;
using HillMetrics.Core.Monitoring;
using HillMetrics.Core.Http.Extensions;
using HillMetrics.MIND.FrontApp.Services;
using HillMetrics.Core.Blazor.AuthModule.AuthHandler;
using HillMetrics.Core.Blazor.AuthModule;
using HillMetrics.MIND.FrontApp.Configs;
using HillMetrics.MIND.Infrastructure.AI;
using HillMetrics.MIND.Infrastructure;
using HillMetrics.Core.AI.Configs;
using ModelContextProtocol.Client;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7098") });

builder.AddHillMetricsCoreExtension();
var logger = builder.InitAndAddHillMetricsLogger(HillMetrics.Orchestrator.ServicesNames.Services.MindFrontApp);

builder.AddHillMetricsServiceDefaults();
builder.Services.ConfigureHillMetricsDefaultHttpClient();

// Configuration des fonctionnalités
builder.Services.Configure<FeaturesConfig>(builder.Configuration.GetSection("Features"));
var featuresConfig = builder.Configuration.GetSection("Features").Get<FeaturesConfig>() ?? new FeaturesConfig();

// Configuration AI
builder.Services.Configure<AiLlmConfig>(builder.Configuration.GetSection("AI"));

builder.Services.AddSingleton<ISignalRNotificationService, SignalRNotificationService>();

var mindApi = builder.Configuration.GetValue<string>("Services:MindApi", $"https+http://{HillMetrics.Orchestrator.ServicesNames.Services.MindAPI}");
//var signalRApi = builder.Configuration.GetValue<string>("Services:SignalRApi", $"https+http://{HillMetrics.Orchestrator.ServicesNames.Services.SignalRService}");
//mindApi = "https://mindapi.hillm.bildhosting.me";

builder.Services.AddHillMetricsHttpClient("MindAPI", client =>
{
    client.BaseAddress = new Uri(mindApi);
    client.Timeout = TimeSpan.FromMinutes(5);
});
//ServicesSettings settings = new ServicesSettings();
//builder.Services.Configure<ServicesSettings>(builder.Configuration.GetSection("Services"));

//builder.Configuration.GetSection("Services").Bind(settings);


builder.Services.Configure<ServicesSettings>(options =>
{
    builder.Configuration.GetSection("Services").Bind(options);
    if (string.IsNullOrEmpty(options.SignalRApi))
        options.SignalRApi = "https://localhost:7228";
});

builder.Services.AddTransient<FileUploadService>();
builder.Services.AddTransient<MappingExportService>();

// Add AI chat services based on configuration
builder.Services.AddAiChatServices(featuresConfig.AiChat.Enabled, builder.Configuration);

builder.Services.AddHillMetricsBlazorMindCookieAuth(builder.Configuration, mindApi, "HillMetrics_MIND", "HillMetrics_MIND");

builder.Services.AddMindApiSDK<AuthenticationHttpHandler>(mindApi, HillMetrics.Orchestrator.ServicesNames.Services.MindFrontApp, TimeSpan.FromMinutes(5));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRazorPages();

// Ajouter services MudBlazor
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.PreventDuplicates = true;
    config.SnackbarConfiguration.NewestOnTop = true;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 5000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 200;
});
builder.Services.AddMudMarkdownServices();



builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddSingleton<IMcpClient>(sp =>
{
    McpClientOptions mcpClientOptions = new()
    { ClientInfo = new() { Name = "AspNetCoreSseClient", Version = "1.0.0" } };

    var client = new HttpClient();
    client.BaseAddress = new($"https+http://{HillMetrics.Orchestrator.ServicesNames.Services.McpFinance}");

    // can't use the service discovery for ["https +http://aspnetsseserver"]
    // fix: read the environment value for the key 'services__aspnetsseserver__https__0' to get the url for the aspnet core sse server
    var serviceName = HillMetrics.Orchestrator.ServicesNames.Services.McpFinance;
    var name = $"services__{serviceName}__https__0";
    var url = Environment.GetEnvironmentVariable(name) + "/sse";

    SseClientTransportOptions sseTransportOptions = new()
    {
        //Endpoint = new Uri("https+http://aspnetsseserver")
        Endpoint = client.BaseAddress
    };

    SseClientTransport sseClientTransport = new(transportOptions: sseTransportOptions);

    var mcpClient = McpClientFactory.CreateAsync(
        sseClientTransport, mcpClientOptions).GetAwaiter().GetResult();
    return mcpClient;
});

var app = builder.Build();

//app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.MapRazorPages();

app.Run();
