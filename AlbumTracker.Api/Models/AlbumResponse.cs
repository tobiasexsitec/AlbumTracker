using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;

namespace AlbumTracker.Api.Models;

/// <summary>Album summary returned from search results.</summary>
public class AlbumResponse
{
    [OpenApiProperty(Description = "Spotify album ID")]
    public string Id { get; set; } = string.Empty;

    [OpenApiProperty(Description = "Album title")]
    public string Name { get; set; } = string.Empty;

    [OpenApiProperty(Description = "Comma-separated artist names")]
    public string Artist { get; set; } = string.Empty;

    [OpenApiProperty(Description = "URL to the album cover image", Nullable = true)]
    public string? CoverImageUrl { get; set; }

    [OpenApiProperty(Description = "Release year extracted from the release date", Nullable = true)]
    public int? ReleaseYear { get; set; }

    [OpenApiProperty(Description = "Spotify album ID (same as Id)", Nullable = true)]
    public string? SpotifyAlbumId { get; set; }
}

/// <summary>Track information within an album.</summary>
public class TrackResponse
{
    [OpenApiProperty(Description = "Track number on the disc")]
    public int Number { get; set; }

    [OpenApiProperty(Description = "Track title")]
    public string Name { get; set; } = string.Empty;

    [OpenApiProperty(Description = "Track duration in milliseconds")]
    public int DurationMs { get; set; }
}

/// <summary>Full album details including track listing.</summary>
public class AlbumDetailsResponse
{
    [OpenApiProperty(Description = "Album metadata")]
    public AlbumResponse Album { get; set; } = new();

    [OpenApiProperty(Description = "List of tracks on the album")]
    public List<TrackResponse> Tracks { get; set; } = [];

    [OpenApiProperty(Description = "Link to the album on Spotify", Nullable = true)]
    public string? ExternalUrl { get; set; }
}
