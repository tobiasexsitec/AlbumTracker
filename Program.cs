using AlbumTracker;
using AlbumTracker.MusicBrainz;
using AlbumTracker.Spotify;
using AlbumTracker.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Album services
builder.Services.AddSingleton<FirebaseJsInterop>();
builder.Services.AddScoped<IAlbumListService, FirebaseAlbumListService>();
builder.Services.AddScoped<IAlbumRatingService, FirebaseAlbumRatingService>();
builder.Services.AddScoped<IListenHistoryService, FirebaseListenHistoryService>();
builder.Services.AddScoped<IAlbumReviewService, FirebaseAlbumReviewService>();
// builder.Services.AddScoped<IAlbumSearchService, MusicBrainzAlbumSearchService>();
builder.Services.AddScoped<IAlbumSearchService, SpotifyAlbumSearchService>();

// Authentication – Firebase Auth with Google
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<FirebaseAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<FirebaseAuthStateProvider>());

var spotifyConfig = builder.Configuration.GetSection("Spotify");
builder.Services.AddSpotify(options =>
{
    options.ClientId = spotifyConfig["ClientId"] ?? string.Empty;
    options.ClientSecret = spotifyConfig["ClientSecret"] ?? string.Empty;
});

var host = builder.Build();

// Initialize the Firebase auth listener before the app renders
var authProvider = host.Services.GetRequiredService<FirebaseAuthStateProvider>();
await authProvider.InitializeAsync();

await host.RunAsync();
