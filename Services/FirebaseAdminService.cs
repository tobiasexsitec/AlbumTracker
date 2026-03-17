using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlbumTracker.Services;

public class FirebaseAdminService : IAdminService
{
    private readonly FirebaseJsInterop _firebase;

    public FirebaseAdminService(FirebaseJsInterop firebase)
    {
        _firebase = firebase;
    }

    public async Task<List<UserProfile>> GetAllUsersAsync()
    {
        try
        {
            var usersDict = await _firebase.GetAsync<Dictionary<string, UserData>>("users");
            if (usersDict is null)
                return [];

            return usersDict.Select(kvp => new UserProfile(
                Uid: kvp.Key,
                DisplayName: kvp.Value.Profile?.DisplayName,
                Email: kvp.Value.Profile?.Email,
                PhotoUrl: kvp.Value.Profile?.PhotoUrl,
                IsAdmin: kvp.Value.IsAdmin ?? false,
                LastSignIn: kvp.Value.LastSignIn
            )).OrderByDescending(u => u.LastSignIn).ToList();
        }
        catch
        {
            return [];
        }
    }

    public async Task SetAdminRoleAsync(string userId, bool isAdmin)
    {
        await _firebase.SetAsync($"users/{userId}/isAdmin", isAdmin);
    }

    private class UserData
    {
        [JsonPropertyName("profile")]
        public ProfileData? Profile { get; set; }

        [JsonPropertyName("isAdmin")]
        public bool? IsAdmin { get; set; }

        [JsonPropertyName("lastSignIn")]
        public DateTime? LastSignIn { get; set; }
    }

    private class ProfileData
    {
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("photoUrl")]
        public string? PhotoUrl { get; set; }
    }
}
