using AlbumTracker.Api.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// Register API core services (includes Spotify) with configuration from app settings
var spotifySection = builder.Configuration.GetSection("Spotify");
builder.Services.AddApiCore(options =>
{
    options.ClientId = spotifySection["ClientId"] ?? string.Empty;
    options.ClientSecret = spotifySection["ClientSecret"] ?? string.Empty;
});

builder.Build().Run();
