using System.Text.Json.Serialization;

namespace AlbumTracker.Spotify.Models;

/// <summary>
/// Represents a paginated list of tracks from the Spotify Web API.
/// </summary>
public class SpotifyTrackPage
{
    [JsonPropertyName("items")]
    public List<SpotifyTrack> Items { get; set; } = [];

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("next")]
    public string? Next { get; set; }
}
