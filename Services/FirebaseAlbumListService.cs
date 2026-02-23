using System.Security.Claims;
using AlbumTracker.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;

namespace AlbumTracker.Services;

public class FirebaseAlbumListService : IAlbumListService
{
    private const string BaseSegment = "albumLists";
    private const string LatestPlayedListName = "Latest Played";

    private readonly FirebaseJsInterop _firebase;
    private readonly FirebaseConfig? _config;
    private readonly AuthenticationStateProvider _authStateProvider;

    public FirebaseAlbumListService(FirebaseJsInterop firebase, IConfiguration configuration, AuthenticationStateProvider authStateProvider)
    {
        _firebase = firebase;
        _config = configuration.GetSection("Firebase").Get<FirebaseConfig>();
        _authStateProvider = authStateProvider;
    }

    private async Task EnsureInitializedAsync()
    {
        if (_config is null)
            throw new InvalidOperationException("Firebase configuration is missing. Ensure the 'Firebase' section exists in appsettings.json.");

        await _firebase.EnsureInitializedAsync(_config);
    }

    private async Task<string> GetBasePathAsync()
    {
        var state = await _authStateProvider.GetAuthenticationStateAsync();
        var uid = state.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User is not authenticated.");
        return $"users/{uid}/{BaseSegment}";
    }

    public async Task<List<AlbumList>> GetAllListsAsync()
    {
        await EnsureInitializedAsync();
        var basePath = await GetBasePathAsync();
        var dict = await _firebase.GetAsync<Dictionary<string, AlbumList>>(basePath);
        return dict?.Values.ToList() ?? [];
    }

    public async Task<AlbumList?> GetListByIdAsync(string listId)
    {
        await EnsureInitializedAsync();
        var basePath = await GetBasePathAsync();
        return await _firebase.GetAsync<AlbumList>($"{basePath}/{listId}");
    }

    public async Task<AlbumList> GetLatestPlayedListAsync()
    {
        await EnsureInitializedAsync();
        var basePath = await GetBasePathAsync();
        var all = await GetAllListsAsync();
        var list = all.FirstOrDefault(l => l.IsDefault);
        if (list is null)
        {
            list = new AlbumList { Name = LatestPlayedListName, IsDefault = true };
            await _firebase.SetAsync($"{basePath}/{list.Id}", list);
        }
        return list;
    }

    public async Task<AlbumList> CreateListAsync(string name)
    {
        await EnsureInitializedAsync();
        var basePath = await GetBasePathAsync();
        var list = new AlbumList { Name = name };
        await _firebase.SetAsync($"{basePath}/{list.Id}", list);
        return list;
    }

    public async Task DeleteListAsync(string listId)
    {
        await EnsureInitializedAsync();
        var basePath = await GetBasePathAsync();
        var list = await GetListByIdAsync(listId);
        if (list is not null && !list.IsDefault)
        {
            await _firebase.RemoveAsync($"{basePath}/{listId}");
        }
    }

    public async Task RenameListAsync(string listId, string newName)
    {
        await EnsureInitializedAsync();
        var basePath = await GetBasePathAsync();
        var list = await GetListByIdAsync(listId);
        if (list is not null)
        {
            list.Name = newName;
            await _firebase.SetAsync($"{basePath}/{listId}", list);
        }
    }

    public async Task AddAlbumToListAsync(string listId, Album album)
    {
        await EnsureInitializedAsync();
        var basePath = await GetBasePathAsync();
        var list = await GetListByIdAsync(listId);
        if (list is null) return;

        if (list.Entries.Any(e => e.Album.Id == album.Id)) return;

        list.Entries.Insert(0, new AlbumListEntry { Album = album });
        await _firebase.SetAsync($"{basePath}/{listId}", list);
    }

    public async Task RemoveAlbumFromListAsync(string listId, string albumId)
    {
        await EnsureInitializedAsync();
        var basePath = await GetBasePathAsync();
        var list = await GetListByIdAsync(listId);
        var entry = list?.Entries.FirstOrDefault(e => e.Album.Id == albumId);
        if (entry is not null)
        {
            list!.Entries.Remove(entry);
            await _firebase.SetAsync($"{basePath}/{listId}", list);
        }
    }
}
