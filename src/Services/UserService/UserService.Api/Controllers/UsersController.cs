using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs;
using UserService.Application.Services;

namespace UserService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserApplicationService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(UserApplicationService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get current user profile.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userService.GetUserByGoogleIdAsync(userId, cancellationToken);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    /// <summary>
    /// Get user by ID.
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetUser(string id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByIdAsync(id, cancellationToken);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    /// <summary>
    /// Create or get user (called after successful authentication).
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateOrGetUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _userService.CreateOrGetUserAsync(request, cancellationToken);
        return Ok(user);
    }

    /// <summary>
    /// Update user profile.
    /// </summary>
    [HttpPut("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> UpdateProfile(
        [FromBody] UpdateProfileRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        // Get user by GoogleId first, then update by actual Id
        var existingUser = await _userService.GetUserByGoogleIdAsync(userId, cancellationToken);
        if (existingUser == null)
            return NotFound();

        var user = await _userService.UpdateProfileAsync(existingUser.Id, request, cancellationToken);
        return Ok(user);
    }

    /// <summary>
    /// Register FCM token for push notifications.
    /// </summary>
    [HttpPost("me/fcm-token")]
    [Authorize]
    public async Task<IActionResult> RegisterFcmToken(
        [FromBody] RegisterFcmTokenRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var existingUser = await _userService.GetUserByGoogleIdAsync(userId, cancellationToken);
        if (existingUser == null)
            return NotFound();

        var success = await _userService.RegisterFcmTokenAsync(existingUser.Id, request, cancellationToken);
        if (!success)
            return BadRequest();

        _logger.LogInformation("FCM token registered for user {UserId}, device {DeviceId}", 
            existingUser.Id, request.DeviceId);
        return NoContent();
    }

    /// <summary>
    /// Remove FCM token.
    /// </summary>
    [HttpDelete("me/fcm-token/{deviceId}")]
    [Authorize]
    public async Task<IActionResult> RemoveFcmToken(string deviceId, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var existingUser = await _userService.GetUserByGoogleIdAsync(userId, cancellationToken);
        if (existingUser == null)
            return NotFound();

        await _userService.RemoveFcmTokenAsync(existingUser.Id, deviceId, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Health check.
    /// </summary>
    [HttpGet("/health")]
    public IActionResult Health() => Ok(new { status = "healthy", service = "user-service" });
}
