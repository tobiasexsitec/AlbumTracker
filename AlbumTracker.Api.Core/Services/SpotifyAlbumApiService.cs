using AlbumTracker.Api.Core.Models;
using AlbumTracker.Spotify.Services;

namespace AlbumTracker.Api.Core.Services;

/// <summary>
/// Implementation of <see cref="IAlbumApiService"/> backed by the Spotify Web API.
/// </summary>
public class SpotifyAlbumApiService : IAlbumApiService
{
    private readonly SpotifyClient _spotifyClient;

    public SpotifyAlbumApiService(SpotifyClient spotifyClient)
    {
        _spotifyClient = spotifyClient;
    }

    public async Task<IReadOnlyList<AlbumResponse>> SearchAlbumsAsync(string query)
    {
        var response = await _spotifyClient.SearchAlbumsAsync(query);
        if (response?.Albums is null)
        {
            return [];
        }

        return response.Albums.Items.Select(a => new AlbumResponse
        {
            Id = a.Id,
            Name = a.Name,
            Artist = string.Join(", ", a.Artists.Select(ar => ar.Name)),
            CoverImageUrl = a.Images.FirstOrDefault()?.Url,
            ReleaseYear = ParseYear(a.ReleaseDate),
            SpotifyAlbumId = a.Id
        }).ToList();
    }

    public async Task<AlbumDetailsResponse?> GetAlbumDetailsAsync(string albumId)
    {
        var spotifyAlbum = await _spotifyClient.GetAlbumAsync(albumId);
        if (spotifyAlbum is null)
        {
            return null;
        }

        return new AlbumDetailsResponse
        {
            Album = new AlbumResponse
            {
                Id = spotifyAlbum.Id,
                Name = spotifyAlbum.Name,
                Artist = string.Join(", ", spotifyAlbum.Artists.Select(ar => ar.Name)),
                CoverImageUrl = spotifyAlbum.Images.FirstOrDefault()?.Url,
                ReleaseYear = ParseYear(spotifyAlbum.ReleaseDate),
                SpotifyAlbumId = spotifyAlbum.Id
            },
            Tracks = spotifyAlbum.Tracks?.Items.Select(t => new TrackResponse
            {
                Number = t.TrackNumber,
                Name = t.Name,
                DurationMs = t.DurationMs
            }).ToList() ?? [],
            ExternalUrl = spotifyAlbum.ExternalUrls?.Spotify
        };
    }

    private static int? ParseYear(string? date)
    {
        if (string.IsNullOrEmpty(date)) return null;
        if (date.Length >= 4 && int.TryParse(date[..4], out var year)) return year;
        return null;
    }
}
