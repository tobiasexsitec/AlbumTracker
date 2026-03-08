using AlbumTracker.Models;

namespace AlbumTracker.Services;

/// <summary>
/// Persists lightweight album metadata so profile pages can
/// display album info without calling external APIs.
/// </summary>
public interface IAlbumInfoService
{
    Task SaveAlbumInfoAsync(AlbumInfo albumInfo);
    Task<AlbumInfo?> GetAlbumInfoAsync(string albumId);
    Task<Dictionary<string, AlbumInfo>> GetAllAlbumInfoAsync();
}
