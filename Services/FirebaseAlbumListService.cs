using AlbumTracker.Models;
using Microsoft.Extensions.Configuration;

namespace AlbumTracker.Services;

public class FirebaseAlbumListService : IAlbumListService
{
    private const string BasePath = "albumLists";
    private const string LatestPlayedListName = "Latest Played";

    private readonly FirebaseJsInterop _firebase;
    private readonly FirebaseConfig _config;

    public FirebaseAlbumListService(FirebaseJsInterop firebase, IConfiguration configuration)
    {
        _firebase = firebase;
        _config = configuration.GetSection("Firebase").Get<FirebaseConfig>()
            ?? throw new InvalidOperationException("Firebase configuration is missing.");
    }

    private async Task EnsureInitializedAsync() => await _firebase.EnsureInitializedAsync(_config);

    public async Task<List<AlbumList>> GetAllListsAsync()
    {
        await EnsureInitializedAsync();
        var dict = await _firebase.GetAsync<Dictionary<string, AlbumList>>(BasePath);
        return dict?.Values.ToList() ?? [];
    }

    public async Task<AlbumList?> GetListByIdAsync(string listId)
    {
        await EnsureInitializedAsync();
        return await _firebase.GetAsync<AlbumList>($"{BasePath}/{listId}");
    }

    public async Task<AlbumList> GetLatestPlayedListAsync()
    {
        await EnsureInitializedAsync();
        var all = await GetAllListsAsync();
        var list = all.FirstOrDefault(l => l.IsDefault);
        if (list is null)
        {
            list = new AlbumList { Name = LatestPlayedListName, IsDefault = true };
            await _firebase.SetAsync($"{BasePath}/{list.Id}", list);
        }
        return list;
    }

    public async Task<AlbumList> CreateListAsync(string name)
    {
        await EnsureInitializedAsync();
        var list = new AlbumList { Name = name };
        await _firebase.SetAsync($"{BasePath}/{list.Id}", list);
        return list;
    }

    public async Task DeleteListAsync(string listId)
    {
        await EnsureInitializedAsync();
        var list = await GetListByIdAsync(listId);
        if (list is not null && !list.IsDefault)
        {
            await _firebase.RemoveAsync($"{BasePath}/{listId}");
        }
    }

    public async Task RenameListAsync(string listId, string newName)
    {
        await EnsureInitializedAsync();
        var list = await GetListByIdAsync(listId);
        if (list is not null)
        {
            list.Name = newName;
            await _firebase.SetAsync($"{BasePath}/{listId}", list);
        }
    }

    public async Task AddAlbumToListAsync(string listId, Album album)
    {
        await EnsureInitializedAsync();
        var list = await GetListByIdAsync(listId);
        if (list is null) return;

        if (list.Entries.Any(e => e.Album.Id == album.Id)) return;

        list.Entries.Insert(0, new AlbumListEntry { Album = album });
        await _firebase.SetAsync($"{BasePath}/{listId}", list);
    }

    public async Task RemoveAlbumFromListAsync(string listId, string albumId)
    {
        await EnsureInitializedAsync();
        var list = await GetListByIdAsync(listId);
        var entry = list?.Entries.FirstOrDefault(e => e.Album.Id == albumId);
        if (entry is not null)
        {
            list!.Entries.Remove(entry);
            await _firebase.SetAsync($"{BasePath}/{listId}", list);
        }
    }
}
