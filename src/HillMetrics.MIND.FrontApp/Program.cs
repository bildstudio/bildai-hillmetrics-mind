using HillMetrics.MIND.FrontApp.Components;
using MudBlazor.Services;
using MudBlazor;
using HillMetrics.MIND.API.SDK;
using HillMetrics.Audit.API.SDK;
using HillMetrics.Core;
using HillMetrics.Core.Monitoring.Logging;
using HillMetrics.Core.Monitoring;
using HillMetrics.Core.Http.Extensions;
using HillMetrics.MIND.FrontApp.Services;
using HillMetrics.Core.Blazor.AuthModule.AuthHandler;
using HillMetrics.Core.Blazor.AuthModule;
using HillMetrics.MIND.FrontApp.Configs;
using HillMetrics.MIND.Infrastructure.AI;
using HillMetrics.Core.AI.Configs;
using HillMetrics.MCP.SDK;

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

// Add MIND API SDK
var mindApi = builder.Configuration.GetValue<string>("Services:MindApi", $"https+http://{HillMetrics.Orchestrator.ServicesNames.Services.MindAPI}");
builder.Services.AddHillMetricsHttpClient("MindAPI", client =>
{
    client.BaseAddress = new Uri(mindApi);
    client.Timeout = TimeSpan.FromMinutes(5);
});
builder.Services.AddMindApiSDK<AuthenticationHttpHandler>(mindApi, HillMetrics.Orchestrator.ServicesNames.Services.MindFrontApp, TimeSpan.FromMinutes(5));

// Add Audit API SDK
var auditApi = builder.Configuration.GetValue<string>("Services:AuditApi", $"https+http://{HillMetrics.Orchestrator.ServicesNames.Services.AuditAPI}");
builder.Services.AddAuditApiSDK<AuthenticationHttpHandler>(auditApi, HillMetrics.Orchestrator.ServicesNames.Services.MindFrontApp, TimeSpan.FromMinutes(5));

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

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure Blazor Server for long-running operations
builder.Services.AddServerSideBlazor(options =>
{
    // Increase circuit timeout to 10 minutes for long-running operations
    options.DetailedErrors = builder.Environment.IsDevelopment();
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(10);
    options.DisconnectedCircuitMaxRetained = 100;
    options.JSInteropDefaultCallTimeout = TimeSpan.FromMinutes(5);
    options.MaxBufferedUnacknowledgedRenderBatches = 50;
});

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

builder.Services.AddMcpFinanceClient(builder.Configuration);
builder.Services.AddMcpMindClient(builder.Configuration);

var app = builder.Build();

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
