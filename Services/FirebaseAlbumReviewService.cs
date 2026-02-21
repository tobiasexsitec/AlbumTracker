using AlbumTracker.Models;
using Microsoft.Extensions.Configuration;

namespace AlbumTracker.Services;

public class FirebaseAlbumReviewService : IAlbumReviewService
{
    private const string BasePath = "albumReviews";

    private readonly FirebaseJsInterop _firebase;
    private readonly FirebaseConfig? _config;

    public FirebaseAlbumReviewService(FirebaseJsInterop firebase, IConfiguration configuration)
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

    public async Task<AlbumReview?> GetReviewAsync(string albumId)
    {
        await EnsureInitializedAsync();
        return await _firebase.GetAsync<AlbumReview>($"{BasePath}/{albumId}");
    }

    public async Task SaveReviewAsync(string albumId, string comment)
    {
        await EnsureInitializedAsync();

        var review = new AlbumReview
        {
            AlbumId = albumId,
            Comment = comment,
            UpdatedAt = DateTime.UtcNow
        };

        await _firebase.SetAsync($"{BasePath}/{albumId}", review);
    }

    public async Task RemoveReviewAsync(string albumId)
    {
        await EnsureInitializedAsync();
        await _firebase.RemoveAsync($"{BasePath}/{albumId}");
    }
}
