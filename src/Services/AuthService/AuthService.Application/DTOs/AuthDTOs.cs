namespace AuthService.Application.DTOs;

/// <summary>
/// Request to sign in with Google ID token.
/// </summary>
public record GoogleSignInRequest(string IdToken);

/// <summary>
/// Authentication response with tokens.
/// </summary>
public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserInfo User
);

/// <summary>
/// Basic user info returned after authentication.
/// </summary>
public record UserInfo(
    string Id,
    string Email,
    string? Name,
    string? Picture
);

/// <summary>
/// Request to refresh access token.
/// </summary>
public record RefreshTokenRequest(string RefreshToken);

/// <summary>
/// Request to revoke refresh token.
/// </summary>
public record RevokeTokenRequest(string RefreshToken);
