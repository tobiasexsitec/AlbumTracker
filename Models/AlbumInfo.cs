namespace AlbumTracker.Models;

/// <summary>
/// Lightweight album metadata stored in the database to avoid
/// repeated external API calls when listing ratings, reviews, etc.
/// </summary>
public class AlbumInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
}
