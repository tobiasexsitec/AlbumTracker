using System.Text.Json.Serialization;

namespace AlbumTracker.Spotify.Models;

/// <summary>
/// Represents an image object from the Spotify Web API.
/// </summary>
public class SpotifyImage
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }
}
