namespace AuthService.Application.Interfaces;

/// <summary>
/// Interface for JWT token generation.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates JWT access token for user.
    /// </summary>
    string GenerateAccessToken(string userId, string email, string? name = null);

    /// <summary>
    /// Generates secure refresh token.
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Gets access token expiration time.
    /// </summary>
    DateTime GetAccessTokenExpiry();
}
