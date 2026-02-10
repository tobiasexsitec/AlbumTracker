namespace AlbumTracker.MusicBrainz.Configuration;

/// <summary>
/// Configuration options for the MusicBrainz API integration.
/// </summary>
public class MusicBrainzOptions
{
    public const string SectionName = "MusicBrainz";

    /// <summary>
    /// The base URL for the MusicBrainz Web API. Defaults to the production endpoint.
    /// </summary>
    public string ApiBaseUrl { get; set; } = "https://musicbrainz.org/ws/2";

    /// <summary>
    /// The base URL for the Cover Art Archive API. Used to fetch album cover images.
    /// </summary>
    public string CoverArtBaseUrl { get; set; } = "https://coverartarchive.org";

    /// <summary>
    /// The application name included in the User-Agent header, as required by MusicBrainz.
    /// </summary>
    public string AppName { get; set; } = "AlbumTracker";

    /// <summary>
    /// The application version included in the User-Agent header.
    /// </summary>
    public string AppVersion { get; set; } = "1.0";

    /// <summary>
    /// The contact email or URL included in the User-Agent header, as required by MusicBrainz.
    /// </summary>
    public string AppContact { get; set; } = string.Empty;
}
