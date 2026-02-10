using AlbumTracker.Spotify.Configuration;
using AlbumTracker.Spotify.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AlbumTracker.Spotify;

/// <summary>
/// Extension methods for registering Spotify services in the dependency injection container.
/// </summary>
public static class SpotifyServiceCollectionExtensions
{
    /// <summary>
    /// Adds Spotify Web API services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure <see cref="SpotifyOptions"/>.</param>
    public static IServiceCollection AddSpotify(this IServiceCollection services, Action<SpotifyOptions> configure)
    {
        services.Configure(configure);

        services.AddScoped<SpotifyAuthService>();
        services.AddScoped<SpotifyClient>();

        return services;
    }
}
