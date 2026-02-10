using System.Text.Json.Serialization;

namespace AlbumTracker.MusicBrainz.Models;

/// <summary>
/// Represents the search response for release groups from the MusicBrainz API.
/// </summary>
public class MusicBrainzReleaseGroupSearchResponse
{
    [JsonPropertyName("release-groups")]
    public List<MusicBrainzReleaseGroup> ReleaseGroups { get; set; } = [];

    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }
}
