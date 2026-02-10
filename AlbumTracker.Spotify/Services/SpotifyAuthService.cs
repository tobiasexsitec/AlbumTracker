using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using AlbumTracker.Spotify.Configuration;
using AlbumTracker.Spotify.Models;
using Microsoft.Extensions.Options;

namespace AlbumTracker.Spotify.Services;

/// <summary>
/// Handles Spotify authentication using the Client Credentials flow.
/// Caches the access token and refreshes it when expired.
/// </summary>
public class SpotifyAuthService
{
    private readonly HttpClient _httpClient;
    private readonly SpotifyOptions _options;
    private string? _accessToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    public SpotifyAuthService(HttpClient httpClient, IOptions<SpotifyOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    /// <summary>
    /// Gets a valid access token, requesting a new one if the current token is expired.
    /// </summary>
    public async Task<string> GetAccessTokenAsync()
    {
        if (_accessToken is not null && DateTime.UtcNow < _tokenExpiry)
        {
            return _accessToken;
        }

        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}"));

        var request = new HttpRequestMessage(HttpMethod.Post, _options.TokenUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials"
        });

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var tokenResponse = await response.Content.ReadFromJsonAsync<SpotifyTokenResponse>();
        if (tokenResponse is null)
        {
            throw new InvalidOperationException("Failed to deserialize Spotify token response.");
        }

        _accessToken = tokenResponse.AccessToken;
        _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60);

        return _accessToken;
    }
}
