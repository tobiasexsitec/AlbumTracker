namespace AlbumTracker.Models;

public class AlbumReview
{
    public string AlbumId { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
