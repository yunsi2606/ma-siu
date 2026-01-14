namespace AuthService.Domain.Entities;

/// <summary>
/// Google authentication result from ID token validation.
/// </summary>
public record GoogleUserInfo(
    string GoogleId,
    string Email,
    string? Name,
    string? Picture,
    bool EmailVerified
);
