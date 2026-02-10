namespace AlbumTracker.Spotify;

/// <summary>
/// Constants for Spotify Web API endpoints and related values.
/// </summary>
public static class SpotifyEndpoints
{
    public const string TokenUrl = "https://accounts.spotify.com/api/token";
    public const string ApiBaseUrl = "https://api.spotify.com/v1";

    public const string SearchPath = "/search";
    public const string AlbumsPath = "/albums";

    /// <summary>
    /// Builds the search URL for a given query.
    /// </summary>
    public static string Search(string query, string type = "album", int limit = 20, int offset = 0)
        => $"{ApiBaseUrl}{SearchPath}?q={Uri.EscapeDataString(query)}&type={type}&limit={limit}&offset={offset}";

    /// <summary>
    /// Builds the album detail URL for a given album ID.
    /// </summary>
    public static string Album(string albumId)
        => $"{ApiBaseUrl}{AlbumsPath}/{Uri.EscapeDataString(albumId)}";

    /// <summary>
    /// Builds the album tracks URL for a given album ID.
    /// </summary>
    public static string AlbumTracks(string albumId, int limit = 50, int offset = 0)
        => $"{ApiBaseUrl}{AlbumsPath}/{Uri.EscapeDataString(albumId)}/tracks?limit={limit}&offset={offset}";
}
