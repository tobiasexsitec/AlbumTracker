namespace AlbumTracker.Models;

public class FeatureRequest
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public string? UserId { get; set; }
    public string? UserEmail { get; set; }
}
