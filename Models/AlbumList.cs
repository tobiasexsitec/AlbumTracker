namespace AlbumTracker.Models;

public class AlbumList
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<AlbumListEntry> Entries { get; set; } = [];
}
