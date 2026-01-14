using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using Google.Apis.Auth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthService.Infrastructure.Google;

/// <summary>
/// Configuration for Google OAuth2.
/// </summary>
public class GoogleAuthOptions
{
    public const string SectionName = "Google";
    
    /// <summary>
    /// Google OAuth2 Client ID.
    /// TODO: Set via configuration/environment variable.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;
}

/// <summary>
/// Google OAuth2 provider implementation.
/// Validates ID tokens using Google's official library.
/// </summary>
public class GoogleAuthProvider : IGoogleAuthProvider
{
    private readonly GoogleAuthOptions _options;
    private readonly ILogger<GoogleAuthProvider> _logger;

    public GoogleAuthProvider(
        IOptions<GoogleAuthOptions> options,
        ILogger<GoogleAuthProvider> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<GoogleUserInfo?> ValidateIdTokenAsync(
        string idToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _options.ClientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            return new GoogleUserInfo(
                GoogleId: payload.Subject,
                Email: payload.Email,
                Name: payload.Name,
                Picture: payload.Picture,
                EmailVerified: payload.EmailVerified
            );
        }
        catch (InvalidJwtException ex)
        {
            _logger.LogWarning(ex, "Invalid Google ID token");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating Google ID token");
            return null;
        }
    }
}
