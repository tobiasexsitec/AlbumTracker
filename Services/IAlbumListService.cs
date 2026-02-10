using AlbumTracker.Models;

namespace AlbumTracker.Services;

public interface IAlbumListService
{
    Task<List<AlbumList>> GetAllListsAsync();
    Task<AlbumList?> GetListByIdAsync(string listId);
    Task<AlbumList> GetLatestPlayedListAsync();
    Task<AlbumList> CreateListAsync(string name);
    Task DeleteListAsync(string listId);
    Task RenameListAsync(string listId, string newName);
    Task AddAlbumToListAsync(string listId, Album album);
    Task RemoveAlbumFromListAsync(string listId, string albumId);
}
