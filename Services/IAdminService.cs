namespace AlbumTracker.Services;

public interface IAdminService
{
    Task<List<UserProfile>> GetAllUsersAsync();
    Task SetAdminRoleAsync(string userId, bool isAdmin);
}

public record UserProfile(
    string Uid,
    string? DisplayName,
    string? Email,
    string? PhotoUrl,
    bool IsAdmin,
    DateTime? LastSignIn
);
