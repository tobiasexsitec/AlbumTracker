using AlbumTracker.Models;
using Microsoft.Extensions.Configuration;

namespace AlbumTracker.Services;

public class FirebaseListenHistoryService : IListenHistoryService
{
    private const string BasePath = "listenHistory";

    private readonly FirebaseJsInterop _firebase;
    private readonly FirebaseConfig? _config;

    public FirebaseListenHistoryService(FirebaseJsInterop firebase, IConfiguration configuration)
    {
        _firebase = firebase;
        _config = configuration.GetSection("Firebase").Get<FirebaseConfig>();
    }

    private async Task EnsureInitializedAsync()
    {
        if (_config is null)
            throw new InvalidOperationException("Firebase configuration is missing. Ensure the 'Firebase' section exists in appsettings.json.");

        await _firebase.EnsureInitializedAsync(_config);
    }

    public async Task<AlbumListenHistory> GetHistoryAsync(string albumId)
    {
        await EnsureInitializedAsync();
        var history = await _firebase.GetAsync<AlbumListenHistory>($"{BasePath}/{albumId}");
        return history ?? new AlbumListenHistory { AlbumId = albumId };
    }

    public async Task AddListenAsync(string albumId, DateOnly listenDate)
    {
        await EnsureInitializedAsync();
        var history = await GetHistoryAsync(albumId);

        history.Entries.Add(new ListenEntry { ListenDate = listenDate });
        await _firebase.SetAsync($"{BasePath}/{albumId}", history);
    }

    public async Task RemoveListenAsync(string albumId, string entryId)
    {
        await EnsureInitializedAsync();
        var history = await _firebase.GetAsync<AlbumListenHistory>($"{BasePath}/{albumId}");

        if (history is not null)
        {
            var entry = history.Entries.FirstOrDefault(e => e.Id == entryId);
            if (entry is not null)
            {
                history.Entries.Remove(entry);
                if (history.Entries.Count == 0)
                    await _firebase.RemoveAsync($"{BasePath}/{albumId}");
                else
                    await _firebase.SetAsync($"{BasePath}/{albumId}", history);
            }
        }
    }
}
