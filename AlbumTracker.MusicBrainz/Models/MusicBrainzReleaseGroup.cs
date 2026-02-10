using System.Text.Json.Serialization;

namespace AlbumTracker.MusicBrainz.Models;

/// <summary>
/// Represents a release group object from the MusicBrainz API.
/// A release group roughly corresponds to an "album" concept.
/// </summary>
public class MusicBrainzReleaseGroup
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("primary-type")]
    public string? PrimaryType { get; set; }

    [JsonPropertyName("secondary-types")]
    public List<string> SecondaryTypes { get; set; } = [];

    [JsonPropertyName("first-release-date")]
    public string? FirstReleaseDate { get; set; }

    [JsonPropertyName("artist-credit")]
    public List<MusicBrainzArtistCredit> ArtistCredit { get; set; } = [];

    [JsonPropertyName("releases")]
    public List<MusicBrainzRelease> Releases { get; set; } = [];
}
