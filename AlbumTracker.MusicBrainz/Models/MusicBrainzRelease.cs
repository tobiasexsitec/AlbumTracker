using System.Text.Json.Serialization;

namespace AlbumTracker.MusicBrainz.Models;

/// <summary>
/// Represents a release (a specific edition of a release group) from the MusicBrainz API.
/// </summary>
public class MusicBrainzRelease
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("date")]
    public string? Date { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("artist-credit")]
    public List<MusicBrainzArtistCredit> ArtistCredit { get; set; } = [];

    [JsonPropertyName("media")]
    public List<MusicBrainzMedia> Media { get; set; } = [];
}
