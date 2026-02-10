using System.Text.Json;
using AlbumTracker.Models;
using Microsoft.JSInterop;

namespace AlbumTracker.Services;

public class LocalStorageAlbumListService : IAlbumListService
{
    private const string StorageKey = "albumtracker_lists";
    private const string LatestPlayedListName = "Latest Played";

    private readonly IJSRuntime _js;
    private List<AlbumList>? _cache;

    public LocalStorageAlbumListService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<List<AlbumList>> GetAllListsAsync()
    {
        await EnsureLoadedAsync();
        return _cache!;
    }

    public async Task<AlbumList?> GetListByIdAsync(string listId)
    {
        await EnsureLoadedAsync();
        return _cache!.FirstOrDefault(l => l.Id == listId);
    }

    public async Task<AlbumList> GetLatestPlayedListAsync()
    {
        await EnsureLoadedAsync();

        var list = _cache!.FirstOrDefault(l => l.IsDefault);
        if (list is null)
        {
            list = new AlbumList { Name = LatestPlayedListName, IsDefault = true };
            _cache!.Add(list);
            await SaveAsync();
        }

        return list;
    }

    public async Task<AlbumList> CreateListAsync(string name)
    {
        await EnsureLoadedAsync();

        var list = new AlbumList { Name = name };
        _cache!.Add(list);
        await SaveAsync();
        return list;
    }

    public async Task DeleteListAsync(string listId)
    {
        await EnsureLoadedAsync();

        var list = _cache!.FirstOrDefault(l => l.Id == listId);
        if (list is not null && !list.IsDefault)
        {
            _cache!.Remove(list);
            await SaveAsync();
        }
    }

    public async Task RenameListAsync(string listId, string newName)
    {
        await EnsureLoadedAsync();

        var list = _cache!.FirstOrDefault(l => l.Id == listId);
        if (list is not null)
        {
            list.Name = newName;
            await SaveAsync();
        }
    }

    public async Task AddAlbumToListAsync(string listId, Album album)
    {
        await EnsureLoadedAsync();

        var list = _cache!.FirstOrDefault(l => l.Id == listId);
        if (list is null) return;

        if (list.Entries.Any(e => e.Album.Id == album.Id)) return;

        list.Entries.Insert(0, new AlbumListEntry { Album = album });
        await SaveAsync();
    }

    public async Task RemoveAlbumFromListAsync(string listId, string albumId)
    {
        await EnsureLoadedAsync();

        var list = _cache!.FirstOrDefault(l => l.Id == listId);
        var entry = list?.Entries.FirstOrDefault(e => e.Album.Id == albumId);
        if (entry is not null)
        {
            list!.Entries.Remove(entry);
            await SaveAsync();
        }
    }

    private async Task EnsureLoadedAsync()
    {
        if (_cache is not null) return;

        var json = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        _cache = string.IsNullOrEmpty(json)
            ? []
            : JsonSerializer.Deserialize<List<AlbumList>>(json) ?? [];
    }

    private async Task SaveAsync()
    {
        var json = JsonSerializer.Serialize(_cache);
        await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
    }
}
