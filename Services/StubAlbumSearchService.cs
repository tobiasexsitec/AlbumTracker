using AlbumTracker.Models;

namespace AlbumTracker.Services;

/// <summary>
/// Stub implementation of <see cref="IAlbumSearchService"/>.
/// Replace this with a real third-party API integration (e.g. MusicBrainz, Spotify, Discogs).
/// </summary>
public class StubAlbumSearchService : IAlbumSearchService
{
    private static readonly List<Album> SampleAlbums =
    [
        new Album
        {
            Id = "1",
            Name = "OK Computer",
            Artist = "Radiohead",
            ReleaseYear = 1997,
            CoverImageUrl = "https://via.placeholder.com/300x300.png?text=OK+Computer",
            Tracks =
            [
                new Track { Number = 1, Name = "Airbag", Duration = TimeSpan.FromMinutes(4.72) },
                new Track { Number = 2, Name = "Paranoid Android", Duration = TimeSpan.FromMinutes(6.42) },
                new Track { Number = 3, Name = "Subterranean Homesick Alien", Duration = TimeSpan.FromMinutes(4.47) },
                new Track { Number = 4, Name = "Exit Music (For a Film)", Duration = TimeSpan.FromMinutes(4.38) },
                new Track { Number = 5, Name = "Let Down", Duration = TimeSpan.FromMinutes(4.92) },
            ]
        },
        new Album
        {
            Id = "2",
            Name = "The Dark Side of the Moon",
            Artist = "Pink Floyd",
            ReleaseYear = 1973,
            CoverImageUrl = "https://via.placeholder.com/300x300.png?text=Dark+Side",
            Tracks =
            [
                new Track { Number = 1, Name = "Speak to Me", Duration = TimeSpan.FromMinutes(1.13) },
                new Track { Number = 2, Name = "Breathe", Duration = TimeSpan.FromMinutes(2.83) },
                new Track { Number = 3, Name = "On the Run", Duration = TimeSpan.FromMinutes(3.55) },
                new Track { Number = 4, Name = "Time", Duration = TimeSpan.FromMinutes(6.88) },
                new Track { Number = 5, Name = "The Great Gig in the Sky", Duration = TimeSpan.FromMinutes(4.72) },
            ]
        },
        new Album
        {
            Id = "3",
            Name = "Abbey Road",
            Artist = "The Beatles",
            ReleaseYear = 1969,
            CoverImageUrl = "https://via.placeholder.com/300x300.png?text=Abbey+Road",
            Tracks =
            [
                new Track { Number = 1, Name = "Come Together", Duration = TimeSpan.FromMinutes(4.33) },
                new Track { Number = 2, Name = "Something", Duration = TimeSpan.FromMinutes(3.05) },
                new Track { Number = 3, Name = "Maxwell's Silver Hammer", Duration = TimeSpan.FromMinutes(3.47) },
                new Track { Number = 4, Name = "Oh! Darling", Duration = TimeSpan.FromMinutes(3.47) },
                new Track { Number = 5, Name = "Octopus's Garden", Duration = TimeSpan.FromMinutes(2.85) },
            ]
        },
        new Album
        {
            Id = "4",
            Name = "Rumours",
            Artist = "Fleetwood Mac",
            ReleaseYear = 1977,
            CoverImageUrl = "https://via.placeholder.com/300x300.png?text=Rumours",
            Tracks =
            [
                new Track { Number = 1, Name = "Second Hand News", Duration = TimeSpan.FromMinutes(2.78) },
                new Track { Number = 2, Name = "Dreams", Duration = TimeSpan.FromMinutes(4.17) },
                new Track { Number = 3, Name = "Never Going Back Again", Duration = TimeSpan.FromMinutes(2.15) },
                new Track { Number = 4, Name = "Don't Stop", Duration = TimeSpan.FromMinutes(3.18) },
                new Track { Number = 5, Name = "Go Your Own Way", Duration = TimeSpan.FromMinutes(3.63) },
            ]
        }
    ];

    public Task<List<Album>> SearchAlbumsAsync(string query)
    {
        var results = SampleAlbums
            .Where(a =>
                a.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                a.Artist.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Task.FromResult(results);
    }

    public Task<AlbumDetailsResult?> GetAlbumDetailsAsync(string albumId)
    {
        var album = SampleAlbums.FirstOrDefault(a => a.Id == albumId);
        var result = album is not null ? new AlbumDetailsResult(album, ExternalUrl: null) : null;
        return Task.FromResult(result);
    }
}
