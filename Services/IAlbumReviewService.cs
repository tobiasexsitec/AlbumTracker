using AlbumTracker.Models;

namespace AlbumTracker.Services;

public interface IAlbumReviewService
{
    Task<AlbumReview?> GetReviewAsync(string albumId);
    Task SaveReviewAsync(string albumId, string comment);
    Task RemoveReviewAsync(string albumId);
    Task<int> GetReviewCountAsync();
}
