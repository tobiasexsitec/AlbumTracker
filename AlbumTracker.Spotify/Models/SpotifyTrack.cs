using System.Text.Json.Serialization;

namespace AlbumTracker.Spotify.Models;

/// <summary>
/// Represents a track object from the Spotify Web API.
/// </summary>
public class SpotifyTrack
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("track_number")]
    public int TrackNumber { get; set; }

    [JsonPropertyName("disc_number")]
    public int DiscNumber { get; set; }

    [JsonPropertyName("duration_ms")]
    public int DurationMs { get; set; }

    [JsonPropertyName("explicit")]
    public bool Explicit { get; set; }

    [JsonPropertyName("artists")]
    public List<SpotifyArtist> Artists { get; set; } = [];

    [JsonPropertyName("external_urls")]
    public SpotifyExternalUrls? ExternalUrls { get; set; }
}
