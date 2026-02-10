using AlbumTracker.Models;

namespace AlbumTracker.Services;

public interface IListenHistoryService
{
    Task<AlbumListenHistory> GetHistoryAsync(string albumId);
    Task AddListenAsync(string albumId, DateOnly listenDate);
    Task RemoveListenAsync(string albumId, string entryId);
}
