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

public class GetAlbumDetailsFunction
{
    private readonly IAlbumApiService _albumApiService;
    private readonly ILogger<GetAlbumDetailsFunction> _logger;

    public GetAlbumDetailsFunction(IAlbumApiService albumApiService, ILogger<GetAlbumDetailsFunction> logger)
    {
        _albumApiService = albumApiService;
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

        var response = await _albumApiService.GetAlbumDetailsAsync(albumId);
        if (response is null)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(response);
    }
}
