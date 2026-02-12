using workvivo_webapp.Components;
using Workvivo.Shared.Configuration;
using Workvivo.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure WorkvivoSettings from configuration
var settings = new WorkvivoSettings();
builder.Configuration.Bind(settings);

// Validate settings (skip if running --help or similar)
try
{
    settings.Validate();
}
catch (InvalidOperationException ex)
{
    Console.Error.WriteLine($"Configuration Error: {ex.Message}");
    Console.Error.WriteLine("\nPlease configure the following:");
    Console.Error.WriteLine("  1. User secrets: dotnet user-secrets set \"ApiToken\" \"your-token\" --project src/workvivo-webapp");
    Console.Error.WriteLine("  2. Or environment variables: WORKVIVO_APITOKEN, WORKVIVO_ORGANIZATIONID");
    Console.Error.WriteLine("  3. Or appsettings.json (not recommended for credentials)");
    Environment.Exit(1);
}

// Register WorkvivoSettings as singleton
builder.Services.AddSingleton(settings);

// Register HttpClient for IWorkvivoApiClient
// Note: WorkvivoApiClient constructor will set headers, so we only configure the factory here
builder.Services.AddHttpClient<IWorkvivoApiClient, WorkvivoApiClient>();

// Add memory cache
builder.Services.AddMemoryCache();

// Register WorkvivoDataService as singleton
builder.Services.AddSingleton<workvivo_webapp.Services.IWorkvivoDataService, workvivo_webapp.Services.WorkvivoDataService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
