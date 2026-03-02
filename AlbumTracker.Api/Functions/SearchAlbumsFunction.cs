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

public class SearchAlbumsFunction
{
    private readonly SpotifyClient _spotifyClient;
    private readonly ILogger<SearchAlbumsFunction> _logger;

    public SearchAlbumsFunction(SpotifyClient spotifyClient, ILogger<SearchAlbumsFunction> logger)
    {
        _spotifyClient = spotifyClient;
        _logger = logger;
    }

    [Function("SearchAlbums")]
    [OpenApiOperation(operationId: "searchAlbums", tags: ["Albums"], Summary = "Search for albums", Description = "Searches Spotify for albums matching the given query string.")]
    [OpenApiParameter(name: "query", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The search query (artist name, album title, etc.)")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<AlbumResponse>), Description = "A list of matching albums")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Missing or empty query parameter")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "albums/search")] HttpRequest req)
    {
        var query = req.Query["query"].ToString();
        if (string.IsNullOrWhiteSpace(query))
        {
            return new BadRequestObjectResult("Query parameter 'query' is required.");
        }

        _logger.LogInformation("Searching albums for query: {Query}", query);

        var response = await _spotifyClient.SearchAlbumsAsync(query);
        if (response?.Albums is null)
        {
            return new OkObjectResult(Array.Empty<AlbumResponse>());
        }

        var albums = response.Albums.Items.Select(a => new AlbumResponse
        {
            Id = a.Id,
            Name = a.Name,
            Artist = string.Join(", ", a.Artists.Select(ar => ar.Name)),
            CoverImageUrl = a.Images.FirstOrDefault()?.Url,
            ReleaseYear = ParseYear(a.ReleaseDate),
            SpotifyAlbumId = a.Id
        }).ToList();

        return new OkObjectResult(albums);
    }

    private static int? ParseYear(string? date)
    {
        if (string.IsNullOrEmpty(date)) return null;
        if (date.Length >= 4 && int.TryParse(date[..4], out var year)) return year;
        return null;
    }
}
