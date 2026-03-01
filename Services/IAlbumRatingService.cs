using AlbumTracker.Models;

namespace AlbumTracker.Services;

public interface IAlbumRatingService
{
    Task<AlbumRating?> GetRatingAsync(string albumId);
    Task SetRatingAsync(string albumId, int rating);
    Task RemoveRatingAsync(string albumId);
    Task<(double Average, int Count)?> GetAverageRatingAsync(string albumId);
    Task<int> GetRatingCountAsync();
    Task<List<AlbumRating>> GetAllRatingsAsync();
}
