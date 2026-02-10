using System.Text.Json.Serialization;

namespace AlbumTracker.MusicBrainz.Models;

/// <summary>
/// Represents a recording object from the MusicBrainz API.
/// </summary>
public class MusicBrainzRecording
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("length")]
    public int? Length { get; set; }

    [JsonPropertyName("disambiguation")]
    public string? Disambiguation { get; set; }
}
