using AlbumTracker.Api.Core.Services;
using AlbumTracker.Spotify;
using AlbumTracker.Spotify.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlbumTracker.Api.Core;

/// <summary>
/// Extension methods for registering AlbumTracker API core services in the dependency injection container.
/// </summary>
public static class ApiCoreServiceCollectionExtensions
{
    /// <summary>
    /// Adds the platform-independent API core services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureSpotify">Action to configure <see cref="SpotifyOptions"/>.</param>
    public static IServiceCollection AddApiCore(this IServiceCollection services, Action<SpotifyOptions> configureSpotify)
    {
        services.AddSpotify(configureSpotify);
        services.AddScoped<IAlbumApiService, SpotifyAlbumApiService>();
        return services;
    }
}
