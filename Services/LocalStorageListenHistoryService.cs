using System.Text.Json;
using AlbumTracker.Models;
using Microsoft.JSInterop;

namespace AlbumTracker.Services;

public class LocalStorageListenHistoryService : IListenHistoryService
{
    private const string StorageKey = "albumtracker_listen_history";

    private readonly IJSRuntime _js;
    private Dictionary<string, AlbumListenHistory>? _cache;

    public LocalStorageListenHistoryService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<AlbumListenHistory> GetHistoryAsync(string albumId)
    {
        await EnsureLoadedAsync();
        return _cache!.TryGetValue(albumId, out var history)
            ? history
            : new AlbumListenHistory { AlbumId = albumId };
    }

    public async Task AddListenAsync(string albumId, DateOnly listenDate)
    {
        await EnsureLoadedAsync();

        if (!_cache!.TryGetValue(albumId, out var history))
        {
            history = new AlbumListenHistory { AlbumId = albumId };
            _cache[albumId] = history;
        }

        history.Entries.Add(new ListenEntry { ListenDate = listenDate });
        await SaveAsync();
    }

    public async Task RemoveListenAsync(string albumId, string entryId)
    {
        await EnsureLoadedAsync();

        if (_cache!.TryGetValue(albumId, out var history))
        {
            var entry = history.Entries.FirstOrDefault(e => e.Id == entryId);
            if (entry is not null)
            {
                history.Entries.Remove(entry);
                if (history.Entries.Count == 0)
                    _cache.Remove(albumId);
                await SaveAsync();
            }
        }
    }

    private async Task EnsureLoadedAsync()
    {
        if (_cache is not null) return;

        var json = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        _cache = string.IsNullOrEmpty(json)
            ? []
            : JsonSerializer.Deserialize<Dictionary<string, AlbumListenHistory>>(json) ?? [];
    }

    private async Task SaveAsync()
    {
        var json = JsonSerializer.Serialize(_cache);
        await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
    }
}
