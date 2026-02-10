using System.Text.Json.Serialization;

namespace AlbumTracker.Spotify.Models;

/// <summary>
/// Represents external URL links from the Spotify Web API.
/// </summary>
public class SpotifyExternalUrls
{
    [JsonPropertyName("spotify")]
    public string? Spotify { get; set; }
}
