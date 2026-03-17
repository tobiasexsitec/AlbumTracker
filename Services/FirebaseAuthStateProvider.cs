using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace AlbumTracker.Services;

public class FirebaseAuthStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly IJSRuntime _js;
    private readonly FirebaseJsInterop _firebase;
    private readonly FirebaseConfig? _config;
    private DotNetObjectReference<FirebaseAuthStateProvider>? _dotNetRef;
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());
    private readonly TaskCompletionSource _initialStateResolved = new();

    public FirebaseAuthStateProvider(IJSRuntime js, FirebaseJsInterop firebase, IConfiguration configuration)
    {
        _js = js;
        _firebase = firebase;
        _config = configuration.GetSection("Firebase").Get<FirebaseConfig>();
    }

    public async Task InitializeAsync()
    {
        if (_config is null)
            throw new InvalidOperationException("Firebase configuration is missing. Ensure the 'Firebase' section exists in appsettings.json.");

        await _firebase.EnsureInitializedAsync(_config);

        _dotNetRef = DotNetObjectReference.Create(this);
        await _js.InvokeVoidAsync("firebaseInterop.onAuthStateChanged", _dotNetRef);
    }

    public async Task SignInWithGoogleAsync()
    {
        await _js.InvokeVoidAsync("firebaseInterop.signInWithGoogle");
    }

    public async Task SignOutAsync()
    {
        await _js.InvokeVoidAsync("firebaseInterop.signOut");
    }

    [JSInvokable]
    public async void OnUserSignedIn(string userJson)
    {
        var user = JsonSerializer.Deserialize<FirebaseUser>(userJson);
        if (user is not null)
        {
            var isAdmin = await CheckIfUserIsAdminAsync(user.Uid);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Uid),
                new(ClaimTypes.Name, user.DisplayName ?? ""),
                new(ClaimTypes.Email, user.Email ?? ""),
                new("picture", user.PhotoUrl ?? "")
            };

            if (isAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            var identity = new ClaimsIdentity(claims, "firebase");
            _currentUser = new ClaimsPrincipal(identity);

            _ = SaveUserProfileAsync(user);
        }

        _initialStateResolved.TrySetResult();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private async Task<bool> CheckIfUserIsAdminAsync(string userId)
    {
        try
        {
            var isAdminValue = await _firebase.GetAsync<bool?>($"users/{userId}/isAdmin");
            return isAdminValue ?? false;
        }
        catch
        {
            // If check fails, default to non-admin
        }
        return false;
    }

    private async Task SaveUserProfileAsync(FirebaseUser user)
    {
        try
        {
            await _firebase.SetAsync($"users/{user.Uid}/profile", new
            {
                displayName = user.DisplayName,
                email = user.Email,
                photoUrl = user.PhotoUrl
            });

            await _firebase.SetAsync($"users/{user.Uid}/lastSignIn", DateTime.UtcNow);
        }
        catch
        {
            // Profile save is best-effort; don't block auth flow
        }
    }

    [JSInvokable]
    public void OnUserSignedOut()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        _initialStateResolved.TrySetResult();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await _initialStateResolved.Task;
        return new AuthenticationState(_currentUser);
    }

    public void Dispose()
    {
        _dotNetRef?.Dispose();
    }
}

public class FirebaseUser
{
    [JsonPropertyName("uid")]
    public string Uid { get; set; } = "";

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("photoUrl")]
    public string? PhotoUrl { get; set; }
}