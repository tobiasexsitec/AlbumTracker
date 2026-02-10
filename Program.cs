using AlbumTracker;
using AlbumTracker.MusicBrainz;
using AlbumTracker.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Album services
builder.Services.AddScoped<IAlbumSearchService, MusicBrainzAlbumSearchService>();
builder.Services.AddScoped<IAlbumListService, LocalStorageAlbumListService>();
builder.Services.AddScoped<IAlbumRatingService, LocalStorageAlbumRatingService>();
builder.Services.AddScoped<IListenHistoryService, LocalStorageListenHistoryService>();

// Authentication is scaffolded but not yet configured.
// When ready, configure Google as the OIDC provider here.
builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Local", options.ProviderOptions);
});

builder.Services.AddMusicBrainz(options =>
{
    options.AppName = "AlbumTracker";
    options.AppVersion = "1.0";
    options.AppContact = "your@email.com";
});

await builder.Build().RunAsync();
