using System.Net.Http.Json;
using AlbumTracker.MusicBrainz.Configuration;
using AlbumTracker.MusicBrainz.Models;
using Microsoft.Extensions.Options;

namespace AlbumTracker.MusicBrainz.Services;

/// <summary>
/// Client for interacting with the MusicBrainz Web API endpoints.
/// MusicBrainz does not require authentication for read-only access,
/// but requires a meaningful User-Agent header for rate limiting identification.
/// </summary>
public class MusicBrainzClient
{
    private readonly HttpClient _httpClient;
    private readonly MusicBrainzOptions _options;

    public MusicBrainzClient(HttpClient httpClient, IOptions<MusicBrainzOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;

        ConfigureUserAgent();
    }

    /// <summary>
    /// Searches for release groups (albums) matching the given query.
    /// </summary>
    /// <param name="query">The search query string.</param>
    /// <param name="limit">Maximum number of results to return (1-100, default 20).</param>
    /// <param name="offset">Index of the first result to return (default 0).</param>
    public async Task<MusicBrainzReleaseGroupSearchResponse?> SearchReleaseGroupsAsync(string query, int limit = 20, int offset = 0)
    {
        var url = $"{_options.ApiBaseUrl}/release-group?query={Uri.EscapeDataString(query)}&limit={limit}&offset={offset}&fmt=json";
        return await _httpClient.GetFromJsonAsync<MusicBrainzReleaseGroupSearchResponse>(url);
    }

    /// <summary>
    /// Gets detailed information about a specific release group, including artist credits.
    /// </summary>
    /// <param name="releaseGroupId">The MusicBrainz release group MBID.</param>
    public async Task<MusicBrainzReleaseGroup?> GetReleaseGroupAsync(string releaseGroupId)
    {
        var url = $"{_options.ApiBaseUrl}/release-group/{Uri.EscapeDataString(releaseGroupId)}?inc=artist-credits&fmt=json";
        return await _httpClient.GetFromJsonAsync<MusicBrainzReleaseGroup>(url);
    }

    /// <summary>
    /// Gets releases for a specific release group, including recordings (tracks) and artist credits.
    /// Returns the first (most relevant) release by default.
    /// </summary>
    /// <param name="releaseGroupId">The MusicBrainz release group MBID.</param>
    /// <param name="limit">Maximum number of releases to return (default 1).</param>
    /// <param name="offset">Index of the first result to return (default 0).</param>
    public async Task<MusicBrainzReleaseBrowseResponse?> GetReleasesForReleaseGroupAsync(string releaseGroupId, int limit = 1, int offset = 0)
    {
        var url = $"{_options.ApiBaseUrl}/release?release-group={Uri.EscapeDataString(releaseGroupId)}&inc=recordings+artist-credits&limit={limit}&offset={offset}&fmt=json";
        return await _httpClient.GetFromJsonAsync<MusicBrainzReleaseBrowseResponse>(url);
    }

    /// <summary>
    /// Gets the front cover art URL for a release group from the Cover Art Archive.
    /// Returns null if no cover art is available.
    /// </summary>
    /// <param name="releaseGroupId">The MusicBrainz release group MBID.</param>
    public string GetCoverArtUrl(string releaseGroupId)
    {
        return $"{_options.CoverArtBaseUrl}/release-group/{Uri.EscapeDataString(releaseGroupId)}/front-250";
    }

    private void ConfigureUserAgent()
    {
        if (_httpClient.DefaultRequestHeaders.UserAgent.Count == 0)
        {
            var userAgent = $"{_options.AppName}/{_options.AppVersion}";
            if (!string.IsNullOrEmpty(_options.AppContact))
            {
                userAgent += $" ({_options.AppContact})";
            }

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
        }
    }
}
