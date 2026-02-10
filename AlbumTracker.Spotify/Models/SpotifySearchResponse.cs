using System.Text.Json.Serialization;

namespace AlbumTracker.Spotify.Models;

/// <summary>
/// Represents the search response from the Spotify Web API.
/// </summary>
public class SpotifySearchResponse
{
    [JsonPropertyName("albums")]
    public SpotifyAlbumPage? Albums { get; set; }
}

public class SpotifyAlbumPage
{
    [JsonPropertyName("items")]
    public List<SpotifyAlbum> Items { get; set; } = [];

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("next")]
    public string? Next { get; set; }
}
