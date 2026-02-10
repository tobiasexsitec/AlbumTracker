using System.Security.Claims;
using System.Text.Json;
using AlbumTracker.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace AlbumTracker.Services;

public class LocalStorageAlbumRatingService : IAlbumRatingService
{
    private const string StorageKeyPrefix = "albumtracker_ratings_";
    private const string AnonymousUser = "anonymous";

    private readonly IJSRuntime _js;
    private readonly AuthenticationStateProvider _authStateProvider;
    private Dictionary<string, AlbumRating>? _cache;
    private string? _cachedUserId;

    public LocalStorageAlbumRatingService(IJSRuntime js, AuthenticationStateProvider authStateProvider)
    {
        _js = js;
        _authStateProvider = authStateProvider;
    }

    public async Task<AlbumRating?> GetRatingAsync(string albumId)
    {
        await EnsureLoadedAsync();
        return _cache!.GetValueOrDefault(albumId);
    }

    public async Task SetRatingAsync(string albumId, int rating)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");

        await EnsureLoadedAsync();

        _cache![albumId] = new AlbumRating
        {
            AlbumId = albumId,
            Rating = rating,
            RatedAt = DateTime.UtcNow
        };

        await SaveAsync();
    }

    public async Task RemoveRatingAsync(string albumId)
    {
        await EnsureLoadedAsync();

        if (_cache!.Remove(albumId))
        {
            await SaveAsync();
        }
    }

    private async Task<string> GetUserIdAsync()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst("sub")?.Value
                ?? user.FindFirst("oid")?.Value
                ?? user.Identity.Name
                ?? AnonymousUser;
        }

        return AnonymousUser;
    }

    private string GetStorageKey(string userId) => $"{StorageKeyPrefix}{userId}";

    private async Task EnsureLoadedAsync()
    {
        var userId = await GetUserIdAsync();

        if (_cache is not null && _cachedUserId == userId)
            return;

        var storageKey = GetStorageKey(userId);
        var json = await _js.InvokeAsync<string?>("localStorage.getItem", storageKey);

        _cache = string.IsNullOrEmpty(json)
            ? []
            : JsonSerializer.Deserialize<Dictionary<string, AlbumRating>>(json) ?? [];

        _cachedUserId = userId;
    }

    private async Task SaveAsync()
    {
        var userId = await GetUserIdAsync();
        var storageKey = GetStorageKey(userId);
        var json = JsonSerializer.Serialize(_cache);
        await _js.InvokeVoidAsync("localStorage.setItem", storageKey, json);
    }
}
