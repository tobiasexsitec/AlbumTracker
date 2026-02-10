using System.Text.Json.Serialization;

namespace AlbumTracker.MusicBrainz.Models;

/// <summary>
/// Represents an artist object from the MusicBrainz API.
/// </summary>
public class MusicBrainzArtist
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("sort-name")]
    public string? SortName { get; set; }

    [JsonPropertyName("disambiguation")]
    public string? Disambiguation { get; set; }
}
