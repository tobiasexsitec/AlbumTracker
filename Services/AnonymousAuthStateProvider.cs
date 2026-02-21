using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace AlbumTracker.Services;

/// <summary>
/// Temporary auth state provider that always returns an anonymous (unauthenticated) user.
/// Replace with real OIDC authentication when ready.
/// </summary>
public class AnonymousAuthStateProvider : AuthenticationStateProvider
{
    private static readonly Task<AuthenticationState> _state =
        Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

    public override Task<AuthenticationState> GetAuthenticationStateAsync() => _state;
}
