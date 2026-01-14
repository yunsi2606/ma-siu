namespace AuthService.Domain.Entities;

/// <summary>
/// Refresh token entity for maintaining user sessions.
/// Stored in MongoDB for token validation and revocation.
/// </summary>
public class RefreshToken
{
    public string Id { get; private set; } = null!;
    public string UserId { get; private set; } = null!;
    public string Token { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }

    private RefreshToken() { }

    public static RefreshToken Create(string userId, string token, int expirationDays = 7)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Token = token,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(expirationDays),
            IsRevoked = false
        };
    }

    public bool IsActive => !IsRevoked && ExpiresAt > DateTime.UtcNow;

    public void Revoke(string? replacedByToken = null)
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        ReplacedByToken = replacedByToken;
    }
}
