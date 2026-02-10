namespace AlbumTracker.Models;

public class AlbumListEntry
{
    public Album Album { get; set; } = new();
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
