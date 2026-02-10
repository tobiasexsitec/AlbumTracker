using System.Text.Json.Serialization;

namespace AlbumTracker.MusicBrainz.Models;

/// <summary>
/// Represents the browse response for releases from the MusicBrainz API.
/// </summary>
public class MusicBrainzReleaseBrowseResponse
{
    [JsonPropertyName("releases")]
    public List<MusicBrainzRelease> Releases { get; set; } = [];

    [JsonPropertyName("release-count")]
    public int ReleaseCount { get; set; }

    [JsonPropertyName("release-offset")]
    public int ReleaseOffset { get; set; }
}
