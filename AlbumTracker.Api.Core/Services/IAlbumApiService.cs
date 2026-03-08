using AlbumTracker.Api.Core.Models;

namespace AlbumTracker.Api.Core.Services;

/// <summary>
/// Platform-independent service for album search and detail retrieval.
/// </summary>
public interface IAlbumApiService
{
    /// <summary>
    /// Searches for albums matching the given query.
    /// </summary>
    Task<IReadOnlyList<AlbumResponse>> SearchAlbumsAsync(string query);

    /// <summary>
    /// Gets detailed information about a specific album, including tracks.
    /// </summary>
    Task<AlbumDetailsResponse?> GetAlbumDetailsAsync(string albumId);
}
