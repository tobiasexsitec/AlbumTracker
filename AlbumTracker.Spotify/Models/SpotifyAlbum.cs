using System.Text.Json.Serialization;

namespace AlbumTracker.Spotify.Models;

/// <summary>
/// Represents an album object from the Spotify Web API.
/// </summary>
public class SpotifyAlbum
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("artists")]
    public List<SpotifyArtist> Artists { get; set; } = [];

    [JsonPropertyName("images")]
    public List<SpotifyImage> Images { get; set; } = [];

    [JsonPropertyName("release_date")]
    public string? ReleaseDate { get; set; }

    [JsonPropertyName("release_date_precision")]
    public string? ReleaseDatePrecision { get; set; }

    [JsonPropertyName("total_tracks")]
    public int TotalTracks { get; set; }

    [JsonPropertyName("album_type")]
    public string? AlbumType { get; set; }

    [JsonPropertyName("tracks")]
    public SpotifyTrackPage? Tracks { get; set; }

    [JsonPropertyName("external_urls")]
    public SpotifyExternalUrls? ExternalUrls { get; set; }
}
