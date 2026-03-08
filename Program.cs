using AlbumTracker;
using AlbumTracker.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Album search via API (Spotify calls are proxied through the Azure Functions backend)
var apiBaseUrl = builder.Configuration["Api:BaseUrl"] ?? builder.HostEnvironment.BaseAddress;
builder.Services.AddHttpClient<IAlbumSearchService, ApiAlbumSearchService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Album services
builder.Services.AddSingleton<FirebaseJsInterop>();
builder.Services.AddScoped<IAlbumInfoService, FirebaseAlbumInfoService>();
builder.Services.AddScoped<IAlbumListService, FirebaseAlbumListService>();
builder.Services.AddScoped<IAlbumRatingService, FirebaseAlbumRatingService>();
builder.Services.AddScoped<IListenHistoryService, FirebaseListenHistoryService>();
builder.Services.AddScoped<IAlbumReviewService, FirebaseAlbumReviewService>();

// Authentication – Firebase Auth with Google
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<FirebaseAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<FirebaseAuthStateProvider>());

var host = builder.Build();

// Initialize the Firebase auth listener before the app renders
var authProvider = host.Services.GetRequiredService<FirebaseAuthStateProvider>();
await authProvider.InitializeAsync();

await host.RunAsync();
