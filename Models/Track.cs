namespace AlbumTracker.Models;

public class Track
{
    public int Number { get; set; }
    public string Name { get; set; } = string.Empty;
    public TimeSpan? Duration { get; set; }
}
