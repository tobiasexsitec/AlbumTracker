using System.Text.Json.Serialization;

namespace AlbumTracker.MusicBrainz.Models;

/// <summary>
/// Represents a media (disc) entry within a release from the MusicBrainz API.
/// </summary>
public class MusicBrainzMedia
{
    [JsonPropertyName("position")]
    public int Position { get; set; }

    [JsonPropertyName("format")]
    public string? Format { get; set; }

    [JsonPropertyName("track-count")]
    public int TrackCount { get; set; }

    [JsonPropertyName("tracks")]
    public List<MusicBrainzTrack> Tracks { get; set; } = [];
}
