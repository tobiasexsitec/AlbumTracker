using System.Net;
using AlbumTracker.Api.Models;
using AlbumTracker.Spotify.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace AlbumTracker.Api.Functions;

public class GetAlbumDetailsFunction
{
    private readonly SpotifyClient _spotifyClient;
    private readonly ILogger<GetAlbumDetailsFunction> _logger;

    public GetAlbumDetailsFunction(SpotifyClient spotifyClient, ILogger<GetAlbumDetailsFunction> logger)
    {
        _spotifyClient = spotifyClient;
        _logger = logger;
    }

    [Function("GetAlbumDetails")]
    [OpenApiOperation(operationId: "getAlbumDetails", tags: ["Albums"], Summary = "Get album details", Description = "Returns full album details including track listing from Spotify.")]
    [OpenApiParameter(name: "albumId", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The Spotify album ID")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(AlbumDetailsResponse), Description = "The album details with tracks")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Missing album ID")]
    [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "Album not found")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "albums/details/{albumId}")] HttpRequest req,
        string albumId)
    {
        if (string.IsNullOrWhiteSpace(albumId))
        {
            return new BadRequestObjectResult("Album ID is required.");
        }

        _logger.LogInformation("Getting album details for: {AlbumId}", albumId);

        var spotifyAlbum = await _spotifyClient.GetAlbumAsync(albumId);
        if (spotifyAlbum is null)
        {
            return new NotFoundResult();
        }

        var response = new AlbumDetailsResponse
        {
            Album = new AlbumResponse
            {
                Id = spotifyAlbum.Id,
                Name = spotifyAlbum.Name,
                Artist = string.Join(", ", spotifyAlbum.Artists.Select(ar => ar.Name)),
                CoverImageUrl = spotifyAlbum.Images.FirstOrDefault()?.Url,
                ReleaseYear = ParseYear(spotifyAlbum.ReleaseDate),
                SpotifyAlbumId = spotifyAlbum.Id
            },
            Tracks = spotifyAlbum.Tracks?.Items.Select(t => new TrackResponse
            {
                Number = t.TrackNumber,
                Name = t.Name,
                DurationMs = t.DurationMs
            }).ToList() ?? [],
            ExternalUrl = spotifyAlbum.ExternalUrls?.Spotify
        };

        return new OkObjectResult(response);
    }

    private static int? ParseYear(string? date)
    {
        if (string.IsNullOrEmpty(date)) return null;
        if (date.Length >= 4 && int.TryParse(date[..4], out var year)) return year;
        return null;
    }
}
