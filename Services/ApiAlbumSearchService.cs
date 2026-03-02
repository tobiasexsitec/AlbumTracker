using System.Net.Http.Json;
using AlbumTracker.Api.Core.Models;
using AlbumTracker.Models;

namespace AlbumTracker.Services;

public class ApiAlbumSearchService : IAlbumSearchService
{
    private readonly HttpClient _httpClient;

    public ApiAlbumSearchService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Album>> SearchAlbumsAsync(string query)
    {
        var response = await _httpClient.GetFromJsonAsync<List<AlbumResponse>>(
            $"api/albums/search?query={Uri.EscapeDataString(query)}");

        if (response is null)
            return [];

        return response.Select(a => new Album
        {
            Id = a.Id,
            Name = a.Name,
            Artist = a.Artist,
            CoverImageUrl = a.CoverImageUrl,
            ReleaseYear = a.ReleaseYear,
            SpotifyAlbumId = a.SpotifyAlbumId
        }).ToList();
    }

    public async Task<AlbumDetailsResult?> GetAlbumDetailsAsync(string albumId)
    {
        var response = await _httpClient.GetFromJsonAsync<AlbumDetailsResponse>(
            $"api/albums/details/{Uri.EscapeDataString(albumId)}");

        if (response?.Album is null)
            return null;

        var album = new Album
        {
            Id = response.Album.Id,
            Name = response.Album.Name,
            Artist = response.Album.Artist,
            CoverImageUrl = response.Album.CoverImageUrl,
            ReleaseYear = response.Album.ReleaseYear,
            SpotifyAlbumId = response.Album.SpotifyAlbumId
        };

        album.Tracks = response.Tracks?.Select(t => new Track
        {
            Number = t.Number,
            Name = t.Name,
            Duration = t.DurationMs > 0 ? TimeSpan.FromMilliseconds(t.DurationMs) : null
        }).ToList() ?? [];

        return new AlbumDetailsResult(album, response.ExternalUrl);
    }
}
