using System.Text.Json.Serialization;

namespace AlbumTracker.MusicBrainz.Models;

/// <summary>
/// Represents an artist credit entry from the MusicBrainz API.
/// </summary>
public class MusicBrainzArtistCredit
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("joinphrase")]
    public string JoinPhrase { get; set; } = string.Empty;

    [JsonPropertyName("artist")]
    public MusicBrainzArtist? Artist { get; set; }
}
