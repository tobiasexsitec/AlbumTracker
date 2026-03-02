using System.Net.Http.Json;
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
        var response = await _httpClient.GetFromJsonAsync<List<AlbumSearchDto>>(
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
        var response = await _httpClient.GetFromJsonAsync<AlbumDetailsDto>(
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

    // DTOs matching the API response shape
    private class AlbumSearchDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public int? ReleaseYear { get; set; }
        public string? SpotifyAlbumId { get; set; }
    }

    private class AlbumDetailsDto
    {
        public AlbumSearchDto Album { get; set; } = new();
        public List<TrackDto> Tracks { get; set; } = [];
        public string? ExternalUrl { get; set; }
    }

    private class TrackDto
    {
        public int Number { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DurationMs { get; set; }
    }
}
