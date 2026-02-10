using System.Text.Json.Serialization;

namespace AlbumTracker.Spotify.Models;

/// <summary>
/// Represents an artist object from the Spotify Web API.
/// </summary>
public class SpotifyArtist
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("external_urls")]
    public SpotifyExternalUrls? ExternalUrls { get; set; }
}
