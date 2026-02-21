using AlbumTracker.Models;
using AlbumTracker.Spotify.Services;

namespace AlbumTracker.Services;

/// <summary>
/// Implementation of <see cref="IAlbumSearchService"/> backed by the Spotify Web API.
/// </summary>
public class SpotifyAlbumSearchService : IAlbumSearchService
{
    private readonly SpotifyClient _client;

    public SpotifyAlbumSearchService(SpotifyClient client)
    {
        _client = client;
    }

    public async Task<List<Album>> SearchAlbumsAsync(string query)
    {
        var response = await _client.SearchAlbumsAsync(query);
        if (response?.Albums is null)
            return [];

        return response.Albums.Items.Select(a => new Album
        {
            Id = a.Id,
            Name = a.Name,
            Artist = string.Join(", ", a.Artists.Select(ar => ar.Name)),
            CoverImageUrl = a.Images.FirstOrDefault()?.Url,
            ReleaseYear = ParseYear(a.ReleaseDate),
            SpotifyAlbumId = a.Id
        }).ToList();
    }

    public async Task<AlbumDetailsResult?> GetAlbumDetailsAsync(string albumId)
    {
        var spotifyAlbum = await _client.GetAlbumAsync(albumId);
        if (spotifyAlbum is null)
            return null;

        var album = new Album
        {
            Id = spotifyAlbum.Id,
            Name = spotifyAlbum.Name,
            Artist = string.Join(", ", spotifyAlbum.Artists.Select(ar => ar.Name)),
            CoverImageUrl = spotifyAlbum.Images.FirstOrDefault()?.Url,
            ReleaseYear = ParseYear(spotifyAlbum.ReleaseDate),
            SpotifyAlbumId = spotifyAlbum.Id
        };

        if (spotifyAlbum.Tracks is not null)
        {
            album.Tracks = spotifyAlbum.Tracks.Items.Select(t => new Track
            {
                Number = t.TrackNumber,
                Name = t.Name,
                Duration = t.DurationMs > 0 ? TimeSpan.FromMilliseconds(t.DurationMs) : null
            }).ToList();
        }

        return new AlbumDetailsResult(album, spotifyAlbum.ExternalUrls?.Spotify);
    }

    private static int? ParseYear(string? date)
    {
        if (string.IsNullOrEmpty(date))
            return null;

        if (date.Length >= 4 && int.TryParse(date[..4], out var year))
            return year;

        return null;
    }
}
