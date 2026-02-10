using AlbumTracker.Models;
using Microsoft.Extensions.Configuration;

namespace AlbumTracker.Services;

public class FirebaseAlbumRatingService : IAlbumRatingService
{
    private const string BasePath = "albumRatings";

    private readonly FirebaseJsInterop _firebase;
    private readonly FirebaseConfig _config;

    public FirebaseAlbumRatingService(FirebaseJsInterop firebase, IConfiguration configuration)
    {
        _firebase = firebase;
        _config = configuration.GetSection("Firebase").Get<FirebaseConfig>()
            ?? throw new InvalidOperationException("Firebase configuration is missing.");
    }

    private async Task EnsureInitializedAsync() => await _firebase.EnsureInitializedAsync(_config);

    public async Task<AlbumRating?> GetRatingAsync(string albumId)
    {
        await EnsureInitializedAsync();
        return await _firebase.GetAsync<AlbumRating>($"{BasePath}/{albumId}");
    }

    public async Task SetRatingAsync(string albumId, int rating)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");

        await EnsureInitializedAsync();

        var albumRating = new AlbumRating
        {
            AlbumId = albumId,
            Rating = rating,
            RatedAt = DateTime.UtcNow
        };

        await _firebase.SetAsync($"{BasePath}/{albumId}", albumRating);
    }

    public async Task RemoveRatingAsync(string albumId)
    {
        await EnsureInitializedAsync();
        await _firebase.RemoveAsync($"{BasePath}/{albumId}");
    }
}
