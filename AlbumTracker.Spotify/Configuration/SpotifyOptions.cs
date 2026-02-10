namespace AlbumTracker.Spotify.Configuration;

/// <summary>
/// Configuration options for the Spotify Web API integration.
/// </summary>
public class SpotifyOptions
{
    public const string SectionName = "Spotify";

    /// <summary>
    /// The Spotify application Client ID.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// The Spotify application Client Secret.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// The base URL for the Spotify Web API. Defaults to the production endpoint.
    /// </summary>
    public string ApiBaseUrl { get; set; } = "https://api.spotify.com/v1";

    /// <summary>
    /// The token endpoint URL used for the Client Credentials flow. Defaults to the production endpoint.
    /// </summary>
    public string TokenUrl { get; set; } = "https://accounts.spotify.com/api/token";
}
