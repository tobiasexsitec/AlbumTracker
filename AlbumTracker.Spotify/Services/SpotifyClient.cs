using System.Net.Http.Headers;
using System.Net.Http.Json;
using AlbumTracker.Spotify.Configuration;
using AlbumTracker.Spotify.Models;
using Microsoft.Extensions.Options;

namespace AlbumTracker.Spotify.Services;

/// <summary>
/// Client for interacting with the Spotify Web API endpoints.
/// </summary>
public class SpotifyClient
{
    private readonly HttpClient _httpClient;
    private readonly SpotifyAuthService _authService;
    private readonly SpotifyOptions _options;

    public SpotifyClient(HttpClient httpClient, SpotifyAuthService authService, IOptions<SpotifyOptions> options)
    {
        _httpClient = httpClient;
        _authService = authService;
        _options = options.Value;
    }

    /// <summary>
    /// Searches for albums matching the given query.
    /// </summary>
    /// <param name="query">The search query string.</param>
    /// <param name="limit">Maximum number of results to return (1-50, default 20).</param>
    /// <param name="offset">Index of the first result to return (default 0).</param>
    public async Task<SpotifySearchResponse?> SearchAlbumsAsync(string query, int limit = 10, int offset = 0)
    {
        await SetAuthHeaderAsync();

        var url = $"{_options.ApiBaseUrl}/search?q={Uri.EscapeDataString(query)}&type=album&limit={limit}&offset={offset}";
        var response = await _httpClient.GetFromJsonAsync<SpotifySearchResponse>(url);
        return response;
    }

    /// <summary>
    /// Gets detailed information about a specific album, including its tracks.
    /// </summary>
    /// <param name="albumId">The Spotify album ID.</param>
    public async Task<SpotifyAlbum?> GetAlbumAsync(string albumId)
    {
        await SetAuthHeaderAsync();

        var url = $"{_options.ApiBaseUrl}/albums/{Uri.EscapeDataString(albumId)}";
        return await _httpClient.GetFromJsonAsync<SpotifyAlbum>(url);
    }

    /// <summary>
    /// Gets the tracks for a specific album.
    /// </summary>
    /// <param name="albumId">The Spotify album ID.</param>
    /// <param name="limit">Maximum number of results to return (1-50, default 50).</param>
    /// <param name="offset">Index of the first result to return (default 0).</param>
    public async Task<SpotifyTrackPage?> GetAlbumTracksAsync(string albumId, int limit = 50, int offset = 0)
    {
        await SetAuthHeaderAsync();

        var url = $"{_options.ApiBaseUrl}/albums/{Uri.EscapeDataString(albumId)}/tracks?limit={limit}&offset={offset}";
        return await _httpClient.GetFromJsonAsync<SpotifyTrackPage>(url);
    }

    private async Task SetAuthHeaderAsync()
    {
        var token = await _authService.GetAccessTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
