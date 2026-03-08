namespace AlbumTracker.Api.Core.Models;

/// <summary>Album summary returned from search results.</summary>
public class AlbumResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public int? ReleaseYear { get; set; }
    public string? SpotifyAlbumId { get; set; }
}

/// <summary>Track information within an album.</summary>
public class TrackResponse
{
    public int Number { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DurationMs { get; set; }
}

/// <summary>Full album details including track listing.</summary>
public class AlbumDetailsResponse
{
    public AlbumResponse Album { get; set; } = new();
    public List<TrackResponse> Tracks { get; set; } = [];
    public string? ExternalUrl { get; set; }
}
