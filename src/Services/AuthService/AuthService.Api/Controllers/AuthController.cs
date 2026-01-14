using AuthService.Application.DTOs;
using AuthService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

/// <summary>
/// Authentication endpoints.
/// Handles Google Sign-In, token refresh, and revocation.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthenticationService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        AuthenticationService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Sign in with Google ID token.
    /// </summary>
    /// <remarks>
    /// Client sends ID token obtained from Google Sign-In.
    /// Returns JWT access token and refresh token.
    /// </remarks>
    [HttpPost("google")]
    public async Task<ActionResult<AuthResponse>> GoogleSignIn(
        [FromBody] GoogleSignInRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.GoogleSignInAsync(request, cancellationToken);

        if (result == null)
        {
            _logger.LogWarning("Failed Google sign-in attempt");
            return Unauthorized(new { message = "Invalid Google ID token" });
        }

        _logger.LogInformation("User {UserId} signed in via Google", result.User.Id);
        return Ok(result);
    }

    /// <summary>
    /// Refresh access token using refresh token.
    /// Uses token rotation for security.
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.RefreshTokenAsync(request, cancellationToken);

        if (result == null)
        {
            _logger.LogWarning("Failed token refresh attempt");
            return Unauthorized(new { message = "Invalid or expired refresh token" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Revoke refresh token (logout).
    /// </summary>
    [HttpPost("revoke")]
    public async Task<IActionResult> RevokeToken(
        [FromBody] RevokeTokenRequest request,
        CancellationToken cancellationToken)
    {
        var success = await _authService.RevokeTokenAsync(request, cancellationToken);

        if (!success)
        {
            return BadRequest(new { message = "Invalid token" });
        }

        return NoContent();
    }

    /// <summary>
    /// Health check.
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "auth-service" });
    }
}
