namespace AlbumTracker.Models;

public class AlbumListenHistory
{
    public string AlbumId { get; set; } = string.Empty;
    public List<ListenEntry> Entries { get; set; } = [];

    public DateOnly? LatestListenDate => Entries.Count > 0
        ? Entries.Max(e => e.ListenDate)
        : null;

    public int TotalListens => Entries.Count;
}

public class ListenEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateOnly ListenDate { get; set; }
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}
