using HillMetrics.MIND.FrontApp.Components;
using HillMetrics.MIND.FrontApp.Services;
using MudBlazor.Services;
using MudBlazor;
using HillMetrics.MIND.FrontApp.Services.Base;
using HillMetrics.MIND.API.SDK;
using HillMetrics.Core;
using HillMetrics.Core.Monitoring.Logging;
using HillMetrics.Core.Monitoring;
using HillMetrics.MIND.API.SDK.V1;
using HillMetrics.Core.Http.Extensions;
using HillMetrics.Core.Blazor.AuthModule.AuthHandler;
using HillMetrics.Core.Blazor.AuthModule;


var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7098") });

builder.AddHillMetricsCoreExtension();
var logger = builder.InitAndAddHillMetricsLogger(HillMetrics.Orchestrator.ServicesNames.Services.MindFrontApp);

builder.AddHillMetricsServiceDefaults();
builder.Services.ConfigureHillMetricsDefaultHttpClient();

var mindApi = builder.Configuration.GetValue<string>("Services:MindApi", $"https+http://{HillMetrics.Orchestrator.ServicesNames.Services.MindAPI}");


builder.Services.AddHillMetricsHttpClient("MindAPI", client =>
{
    client.BaseAddress = new Uri(mindApi);
    client.Timeout = TimeSpan.FromMinutes(5);
});

builder.Services.AddHillMetricsBlazorCookieAuth(mindApi, "HillMetrics_MIND", "HillMetrics_MIND");

builder.Services.AddMindApiSDK<AuthenticationHttpHandler>(mindApi, "Mind-frontend");

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

// Register services for API communication
builder.Services.AddScoped<IFluxService, FluxService>();
builder.Services.AddScoped<ISourceService, SourceService>();



builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCascadingAuthenticationState();

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
