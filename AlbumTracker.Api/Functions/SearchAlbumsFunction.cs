using System.Net;
using AlbumTracker.Api.Core.Models;
using AlbumTracker.Api.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace AlbumTracker.Api.Functions;

public class SearchAlbumsFunction
{
    private readonly IAlbumApiService _albumApiService;
    private readonly ILogger<SearchAlbumsFunction> _logger;

    public SearchAlbumsFunction(IAlbumApiService albumApiService, ILogger<SearchAlbumsFunction> logger)
    {
        _albumApiService = albumApiService;
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

        var albums = await _albumApiService.SearchAlbumsAsync(query);
        return new OkObjectResult(albums);
    }
}
