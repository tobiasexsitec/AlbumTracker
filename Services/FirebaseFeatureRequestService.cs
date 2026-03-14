using System.Security.Claims;
using AlbumTracker.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;

namespace AlbumTracker.Services;

public class FirebaseFeatureRequestService : IFeatureRequestService
{
    private const string BaseSegment = "featureRequests";

    private readonly FirebaseJsInterop _firebase;
    private readonly FirebaseConfig? _config;
    private readonly AuthenticationStateProvider _authStateProvider;

    public FirebaseFeatureRequestService(FirebaseJsInterop firebase, IConfiguration configuration, AuthenticationStateProvider authStateProvider)
    {
        _firebase = firebase;
        _config = configuration.GetSection("Firebase").Get<FirebaseConfig>();
        _authStateProvider = authStateProvider;
    }

    private async Task EnsureInitializedAsync()
    {
        if (_config is null)
            throw new InvalidOperationException("Firebase configuration is missing. Ensure the 'Firebase' section exists in appsettings.json.");

        await _firebase.EnsureInitializedAsync(_config);
    }

    public async Task SubmitFeatureRequestAsync(string title, string description)
    {
        await EnsureInitializedAsync();

        var featureRequest = new FeatureRequest
        {
            Id = Guid.NewGuid().ToString(),
            Title = title,
            Description = description,
            SubmittedAt = DateTime.UtcNow
        };

        // Try to get user info if authenticated (but don't fail if not)
        try
        {
            var state = await _authStateProvider.GetAuthenticationStateAsync();
            if (state?.User?.Identity?.IsAuthenticated == true)
            {
                featureRequest.UserId = state.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                featureRequest.UserEmail = state.User.FindFirst(ClaimTypes.Email)?.Value;
            }
        }
        catch
        {
            // Ignore authentication errors for anonymous users
        }

        // Store in a global collection (not per-user)
        await _firebase.SetAsync($"{BaseSegment}/{featureRequest.Id}", featureRequest);
    }

    public async Task<List<FeatureRequest>> GetAllFeatureRequestsAsync()
    {
        await EnsureInitializedAsync();
        var allRequests = await _firebase.GetAsync<Dictionary<string, FeatureRequest>>(BaseSegment);
        return allRequests?.Values.OrderByDescending(r => r.SubmittedAt).ToList() ?? [];
    }
}
