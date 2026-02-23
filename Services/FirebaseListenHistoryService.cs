using System.Security.Claims;
using AlbumTracker.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;

namespace AlbumTracker.Services;

public class FirebaseListenHistoryService : IListenHistoryService
{
    private const string BaseSegment = "listenHistory";

    private readonly FirebaseJsInterop _firebase;
    private readonly FirebaseConfig? _config;
    private readonly AuthenticationStateProvider _authStateProvider;

    public FirebaseListenHistoryService(FirebaseJsInterop firebase, IConfiguration configuration, AuthenticationStateProvider authStateProvider)
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

    public async Task<AlbumListenHistory> GetHistoryAsync(string albumId)
    {
        await EnsureInitializedAsync();
        var basePath = await GetBasePathAsync();
        var history = await _firebase.GetAsync<AlbumListenHistory>($"{basePath}/{albumId}");
        return history ?? new AlbumListenHistory { AlbumId = albumId };
    }

    public async Task AddListenAsync(string albumId, DateOnly listenDate)
    {
        await EnsureInitializedAsync();
        var basePath = await GetBasePathAsync();
        var history = await GetHistoryAsync(albumId);

        history.Entries.Add(new ListenEntry { ListenDate = listenDate });
        await _firebase.SetAsync($"{basePath}/{albumId}", history);
    }

    public async Task RemoveListenAsync(string albumId, string entryId)
    {
        await EnsureInitializedAsync();
        var basePath = await GetBasePathAsync();
        var history = await _firebase.GetAsync<AlbumListenHistory>($"{basePath}/{albumId}");

        if (history is not null)
        {
            var entry = history.Entries.FirstOrDefault(e => e.Id == entryId);
            if (entry is not null)
            {
                history.Entries.Remove(entry);
                if (history.Entries.Count == 0)
                    await _firebase.RemoveAsync($"{basePath}/{albumId}");
                else
                    await _firebase.SetAsync($"{basePath}/{albumId}", history);
            }
        }
    }
}
