namespace AlbumTracker.Models;

public class Album
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public int? ReleaseYear { get; set; }
    public string? SpotifyAlbumId { get; set; }
    public List<Track> Tracks { get; set; } = [];
}
