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

    public async Task<FirebaseUser?> SignInWithGoogleAsync()
    {
        var json = await _js.InvokeAsync<string>("firebaseInterop.signInWithGoogle");
        var user = JsonSerializer.Deserialize<FirebaseUser>(json);
        return user;
    }

    public async Task SignOutAsync()
    {
        await _js.InvokeVoidAsync("firebaseInterop.signOut");
    }

    [JSInvokable]
    public void OnUserSignedIn(string userJson)
    {
        var user = JsonSerializer.Deserialize<FirebaseUser>(userJson);
        if (user is not null)
        {
            var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, user.Uid),
                new Claim(ClaimTypes.Name, user.DisplayName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("picture", user.PhotoUrl ?? "")
            ], "firebase");

            _currentUser = new ClaimsPrincipal(identity);
        }

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    [JSInvokable]
    public void OnUserSignedOut()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
        => Task.FromResult(new AuthenticationState(_currentUser));

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