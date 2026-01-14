using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces;

/// <summary>
/// Interface for Google OAuth2 authentication.
/// </summary>
public interface IGoogleAuthProvider
{
    /// <summary>
    /// Validates Google ID token and returns user info.
    /// </summary>
    /// <param name="idToken">ID token from Google Sign-In</param>
    /// <returns>User info if valid, null otherwise</returns>
    Task<GoogleUserInfo?> ValidateIdTokenAsync(string idToken, CancellationToken cancellationToken = default);
}
