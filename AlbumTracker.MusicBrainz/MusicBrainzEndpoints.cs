namespace AlbumTracker.MusicBrainz;

/// <summary>
/// Constants for MusicBrainz API endpoints and related values.
/// </summary>
public static class MusicBrainzEndpoints
{
    public const string ApiBaseUrl = "https://musicbrainz.org/ws/2";
    public const string CoverArtBaseUrl = "https://coverartarchive.org";

    public const string ReleaseGroupPath = "/release-group";
    public const string ReleasePath = "/release";

    /// <summary>
    /// Builds the search URL for release groups matching a given query.
    /// </summary>
    public static string SearchReleaseGroups(string query, int limit = 20, int offset = 0)
        => $"{ApiBaseUrl}{ReleaseGroupPath}?query={Uri.EscapeDataString(query)}&limit={limit}&offset={offset}&fmt=json";

    /// <summary>
    /// Builds the lookup URL for a specific release group, including artist credits.
    /// </summary>
    public static string ReleaseGroup(string mbid)
        => $"{ApiBaseUrl}{ReleaseGroupPath}/{Uri.EscapeDataString(mbid)}?inc=artist-credits&fmt=json";

    /// <summary>
    /// Builds the browse URL for releases belonging to a release group, including recordings and artist credits.
    /// </summary>
    public static string ReleasesForReleaseGroup(string releaseGroupMbid, int limit = 1, int offset = 0)
        => $"{ApiBaseUrl}{ReleasePath}?release-group={Uri.EscapeDataString(releaseGroupMbid)}&inc=recordings+artist-credits&limit={limit}&offset={offset}&fmt=json";

    /// <summary>
    /// Builds the Cover Art Archive URL to get front cover art for a release group.
    /// </summary>
    public static string CoverArtFront(string releaseGroupMbid)
        => $"{CoverArtBaseUrl}/release-group/{Uri.EscapeDataString(releaseGroupMbid)}/front-250";
}
