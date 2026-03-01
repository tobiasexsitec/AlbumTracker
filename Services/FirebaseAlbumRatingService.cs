using System.Security.Claims;
using AlbumTracker.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;

namespace AlbumTracker.Services;

public class FirebaseAlbumRatingService : IAlbumRatingService
{
    private const string BaseSegment = "albumRatings";
    private const string SharedRatingsSegment = "ratings";

    private readonly FirebaseJsInterop _firebase;
    private readonly FirebaseConfig? _config;
    private readonly AuthenticationStateProvider _authStateProvider;

    public FirebaseAlbumRatingService(FirebaseJsInterop firebase, IConfiguration configuration, AuthenticationStateProvider authStateProvider)
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

    public async Task<AlbumRating?> GetRatingAsync(string albumId)
    {
        await EnsureInitializedAsync();
        var basePath = await GetBasePathAsync();
        return await _firebase.GetAsync<AlbumRating>($"{basePath}/{albumId}");
    }

    public async Task SetRatingAsync(string albumId, int rating)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");

        await EnsureInitializedAsync();
        var basePath = await GetBasePathAsync();
        var userId = await GetUserIdAsync();

        var albumRating = new AlbumRating
        {
            AlbumId = albumId,
            Rating = rating,
            RatedAt = DateTime.UtcNow
        };

        await _firebase.SetAsync($"{basePath}/{albumId}", albumRating);
        await _firebase.SetAsync($"{SharedRatingsSegment}/{albumId}/{userId}", rating);
    }

    public async Task RemoveRatingAsync(string albumId)
    {
        await EnsureInitializedAsync();
        var basePath = await GetBasePathAsync();
        var userId = await GetUserIdAsync();
        await _firebase.RemoveAsync($"{basePath}/{albumId}");
        await _firebase.RemoveAsync($"{SharedRatingsSegment}/{albumId}/{userId}");
    }

    public async Task<(double Average, int Count)?> GetAverageRatingAsync(string albumId)
    {
        await EnsureInitializedAsync();
        var allRatings = await _firebase.GetAsync<Dictionary<string, int>>($"{SharedRatingsSegment}/{albumId}");
        if (allRatings is null || allRatings.Count == 0)
            return null;

        var average = allRatings.Values.Average();
        return (average, allRatings.Count);
    }

    private async Task<string> GetUserIdAsync()
    {
        var state = await _authStateProvider.GetAuthenticationStateAsync();
        return state.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new InvalidOperationException("User is not authenticated.");
    }
}
