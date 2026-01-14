namespace UserService.Domain.Entities;

/// <summary>
/// User entity representing app users.
/// Created when user first signs in via Google OAuth.
/// </summary>
public class User
{
    public string Id { get; private set; } = null!;
    public string GoogleId { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? DisplayName { get; private set; }
    public string? AvatarUrl { get; private set; }
    public UserRole Role { get; private set; } = UserRole.User;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsActive { get; private set; } = true;
    
    // FCM tokens for push notifications (multiple devices)
    private readonly List<FcmToken> _fcmTokens = new();
    public IReadOnlyCollection<FcmToken> FcmTokens => _fcmTokens.AsReadOnly();
    
    // Points system
    public int TotalPoints { get; private set; }
    public int PendingPoints { get; private set; }

    private User() { }

    public static User Create(string googleId, string email, string? displayName = null, string? avatarUrl = null)
    {
        return new User
        {
            Id = Guid.NewGuid().ToString(),
            GoogleId = googleId,
            Email = email,
            DisplayName = displayName,
            AvatarUrl = avatarUrl,
            CreatedAt = DateTime.UtcNow,
            TotalPoints = 0,
            PendingPoints = 0
        };
    }

    public void UpdateProfile(string? displayName, string? avatarUrl)
    {
        DisplayName = displayName;
        AvatarUrl = avatarUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddFcmToken(string token, string deviceId, string? platform = null)
    {
        // Remove existing token for this device
        _fcmTokens.RemoveAll(t => t.DeviceId == deviceId);
        
        _fcmTokens.Add(new FcmToken(token, deviceId, platform));
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveFcmToken(string deviceId)
    {
        _fcmTokens.RemoveAll(t => t.DeviceId == deviceId);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddPoints(int points, bool isPending = false)
    {
        if (isPending)
            PendingPoints += points;
        else
            TotalPoints += points;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ApprovePendingPoints(int points)
    {
        if (points > PendingPoints)
            throw new InvalidOperationException("Cannot approve more points than pending");
        
        PendingPoints -= points;
        TotalPoints += points;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetRole(UserRole role)
    {
        Role = role;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum UserRole
{
    User = 0,
    Admin = 1
}

/// <summary>
/// FCM token for push notifications.
/// </summary>
public record FcmToken(
    string Token,
    string DeviceId,
    string? Platform = null,
    DateTime CreatedAt = default
)
{
    public DateTime CreatedAt { get; init; } = CreatedAt == default ? DateTime.UtcNow : CreatedAt;
}
