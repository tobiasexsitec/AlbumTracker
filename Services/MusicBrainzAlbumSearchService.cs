using AlbumTracker.Models;
using AlbumTracker.MusicBrainz.Services;

namespace AlbumTracker.Services;

/// <summary>
/// Implementation of <see cref="IAlbumSearchService"/> backed by the MusicBrainz API.
/// </summary>
public class MusicBrainzAlbumSearchService : IAlbumSearchService
{
    private readonly MusicBrainzClient _client;

    public MusicBrainzAlbumSearchService(MusicBrainzClient client)
    {
        _client = client;
    }

    public async Task<List<Album>> SearchAlbumsAsync(string query)
    {
        var response = await _client.SearchReleaseGroupsAsync(query);
        if (response is null)
            return [];

        var res = response.ReleaseGroups.Select(rg => new Album
        {
            Id = rg.Id,
            Name = rg.Title,
            Artist = string.Join(", ", rg.ArtistCredit.Select(ac => ac.Name)),
            CoverImageUrl = _client.GetCoverArtUrl(rg.Id),
            ReleaseYear = ParseYear(rg.FirstReleaseDate)
        }).ToList();
        return res;
    }

    public async Task<AlbumDetailsResult?> GetAlbumDetailsAsync(string albumId)
    {
        var releaseGroup = await _client.GetReleaseGroupAsync(albumId);
        if (releaseGroup is null)
            return null;

        var album = new Album
        {
            Id = releaseGroup.Id,
            Name = releaseGroup.Title,
            Artist = string.Join(", ", releaseGroup.ArtistCredit.Select(ac => ac.Name)),
            CoverImageUrl = _client.GetCoverArtUrl(releaseGroup.Id),
            ReleaseYear = ParseYear(releaseGroup.FirstReleaseDate)
        };

        // Fetch the first release to get tracks
        var releases = await _client.GetReleasesForReleaseGroupAsync(albumId);
        var release = releases?.Releases.FirstOrDefault();
        if (release is not null)
        {
            album.Tracks = release.Media
                .SelectMany(m => m.Tracks)
                .Select(t => new Track
                {
                    Number = t.Position,
                    Name = t.Title,
                    Duration = t.Length is > 0 ? TimeSpan.FromMilliseconds(t.Length.Value) : null
                })
                .ToList();
        }

        return new AlbumDetailsResult(album, ExternalUrl: null);
    }

    private static int? ParseYear(string? date)
    {
        if (string.IsNullOrEmpty(date))
            return null;

        // MusicBrainz dates can be "2024", "2024-01", or "2024-01-15"
        if (date.Length >= 4 && int.TryParse(date[..4], out var year))
            return year;

        return null;
    }
}
