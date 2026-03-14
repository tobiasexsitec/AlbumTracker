using AlbumTracker.Models;

namespace AlbumTracker.Services;

public interface IFeatureRequestService
{
    Task SubmitFeatureRequestAsync(string title, string description);
    Task<List<FeatureRequest>> GetAllFeatureRequestsAsync();
}
