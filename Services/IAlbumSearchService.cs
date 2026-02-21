using AlbumTracker.Models;

namespace AlbumTracker.Services;

public interface IAlbumSearchService
{
    Task<List<Album>> SearchAlbumsAsync(string query);
    Task<AlbumDetailsResult?> GetAlbumDetailsAsync(string albumId);
}
