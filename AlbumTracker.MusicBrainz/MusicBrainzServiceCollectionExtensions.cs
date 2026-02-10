using AlbumTracker.MusicBrainz.Configuration;
using AlbumTracker.MusicBrainz.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AlbumTracker.MusicBrainz;

/// <summary>
/// Extension methods for registering MusicBrainz services in the dependency injection container.
/// </summary>
public static class MusicBrainzServiceCollectionExtensions
{
    /// <summary>
    /// Adds MusicBrainz API services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure <see cref="MusicBrainzOptions"/>.</param>
    public static IServiceCollection AddMusicBrainz(this IServiceCollection services, Action<MusicBrainzOptions> configure)
    {
        services.Configure(configure);

        services.AddScoped<MusicBrainzClient>();

        return services;
    }
}
