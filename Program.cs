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
// LocalStorage services (used when Firebase is disconnected):
// builder.Services.AddScoped<IAlbumListService, LocalStorageAlbumListService>();
// builder.Services.AddScoped<IAlbumRatingService, LocalStorageAlbumRatingService>();
// builder.Services.AddScoped<IListenHistoryService, LocalStorageListenHistoryService>();
// builder.Services.AddScoped<IAlbumReviewService, LocalStorageAlbumReviewService>();

// Authentication is scaffolded but not yet configured.
// When ready, configure Google as the OIDC provider here.
// OIDC temporarily disconnected for debugging:
// builder.Services.AddOidcAuthentication(options =>
// {
//     builder.Configuration.Bind("Local", options.ProviderOptions);
// });
builder.Services.AddAuthorizationCore();
builder.Services.AddSingleton<AuthenticationStateProvider, AnonymousAuthStateProvider>();

// builder.Services.AddMusicBrainz(options =>
// {
//     options.AppName = "AlbumTracker";
//     options.AppVersion = "1.0";
//     options.AppContact = "your@email.com";
// });

var spotifyConfig = builder.Configuration.GetSection("Spotify");
builder.Services.AddSpotify(options =>
{
    options.ClientId = spotifyConfig["ClientId"] ?? string.Empty;
    options.ClientSecret = spotifyConfig["ClientSecret"] ?? string.Empty;
});

await builder.Build().RunAsync();
