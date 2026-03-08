using AlbumTracker.Models;
using Microsoft.Extensions.Configuration;

namespace AlbumTracker.Services;

public class FirebaseAlbumInfoService : IAlbumInfoService
{
    private const string BasePath = "albumInfo";

    private readonly FirebaseJsInterop _firebase;
    private readonly FirebaseConfig? _config;

    public FirebaseAlbumInfoService(FirebaseJsInterop firebase, IConfiguration configuration)
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

    public async Task SaveAlbumInfoAsync(AlbumInfo albumInfo)
    {
        await EnsureInitializedAsync();
        await _firebase.SetAsync($"{BasePath}/{albumInfo.Id}", albumInfo);
    }

    public async Task<AlbumInfo?> GetAlbumInfoAsync(string albumId)
    {
        await EnsureInitializedAsync();
        return await _firebase.GetAsync<AlbumInfo>($"{BasePath}/{albumId}");
    }

    public async Task<Dictionary<string, AlbumInfo>> GetAllAlbumInfoAsync()
    {
        await EnsureInitializedAsync();
        var all = await _firebase.GetAsync<Dictionary<string, AlbumInfo>>(BasePath);
        return all ?? [];
    }
}
