using System.Security.Claims;
using AlbumTracker.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;

namespace AlbumTracker.Services;

public class FirebaseAlbumReviewService : IAlbumReviewService
{
    private const string BaseSegment = "albumReviews";

    private readonly FirebaseJsInterop _firebase;
    private readonly FirebaseConfig? _config;
    private readonly AuthenticationStateProvider _authStateProvider;

    public FirebaseAlbumReviewService(FirebaseJsInterop firebase, IConfiguration configuration, AuthenticationStateProvider authStateProvider)
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

    public async Task<AlbumReview?> GetReviewAsync(string albumId)
    {
        await EnsureInitializedAsync();
        var basePath = await GetBasePathAsync();
        return await _firebase.GetAsync<AlbumReview>($"{basePath}/{albumId}");
    }

    public async Task SaveReviewAsync(string albumId, string comment)
    {
        await EnsureInitializedAsync();
        var basePath = await GetBasePathAsync();

        var review = new AlbumReview
        {
            AlbumId = albumId,
            Comment = comment,
            UpdatedAt = DateTime.UtcNow
        };

        await _firebase.SetAsync($"{basePath}/{albumId}", review);
    }

    public async Task RemoveReviewAsync(string albumId)
    {
        await EnsureInitializedAsync();
        var basePath = await GetBasePathAsync();
        await _firebase.RemoveAsync($"{basePath}/{albumId}");
    }
}
