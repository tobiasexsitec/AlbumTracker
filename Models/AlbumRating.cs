namespace AlbumTracker.Models;

public class AlbumRating
{
    public string AlbumId { get; set; } = string.Empty;
    public int Rating { get; set; }
    public DateTime RatedAt { get; set; } = DateTime.UtcNow;
}
