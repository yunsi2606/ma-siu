using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.Interfaces;
using NotificationService.Application.Services;
using NotificationService.Domain.Entities;

namespace NotificationService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly NotificationSenderService _senderService;
    private readonly INotificationRepository _repository;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        NotificationSenderService senderService,
        INotificationRepository repository,
        ILogger<NotificationsController> logger)
    {
        _senderService = senderService;
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Get user's notifications.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<NotificationsResponse>> GetNotifications(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var skip = (page - 1) * pageSize;
        var notifications = await _repository.GetByUserIdAsync(userId, skip, pageSize, cancellationToken);
        var unreadCount = await _repository.GetUnreadCountAsync(userId, cancellationToken);

        return Ok(new NotificationsResponse(
            notifications.Select(MapToDto).ToList(),
            unreadCount,
            page,
            pageSize
        ));
    }

    /// <summary>
    /// Get unread count.
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<ActionResult<UnreadCountResponse>> GetUnreadCount(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var count = await _repository.GetUnreadCountAsync(userId, cancellationToken);
        return Ok(new UnreadCountResponse(count));
    }

    /// <summary>
    /// Mark notification as read.
    /// </summary>
    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkRead(string id, CancellationToken cancellationToken)
    {
        var notification = await _repository.GetByIdAsync(id, cancellationToken);
        if (notification == null) return NotFound();

        notification.MarkRead();
        await _repository.UpdateAsync(notification, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Mark all notifications as read.
    /// </summary>
    [HttpPost("mark-all-read")]
    public async Task<IActionResult> MarkAllRead(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await _repository.MarkAllReadAsync(userId, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Send notification (internal API - for other services).
    /// </summary>
    [HttpPost("send")]
    [AllowAnonymous] // Internal service-to-service call
    public async Task<IActionResult> Send([FromBody] SendNotificationRequest request, CancellationToken cancellationToken)
    {
        // Validate internal API key
        var apiKey = Request.Headers["X-Internal-Api-Key"].FirstOrDefault();
        if (apiKey != "masiu-internal-key") // TODO: Move to config
        {
            return Unauthorized();
        }

        var success = await _senderService.SendAsync(
            request.UserId,
            request.FcmTokens,
            Enum.Parse<NotificationType>(request.Type),
            request.Title,
            request.Body,
            request.ImageUrl,
            request.Data,
            request.DeduplicationKey,
            cancellationToken);

        return success ? Ok() : BadRequest(new { message = "Notification send failed or deduplicated" });
    }

    /// <summary>
    /// Send to topic (broadcast).
    /// </summary>
    [HttpPost("send-topic")]
    [AllowAnonymous]
    public async Task<IActionResult> SendToTopic([FromBody] SendTopicRequest request, CancellationToken cancellationToken)
    {
        var apiKey = Request.Headers["X-Internal-Api-Key"].FirstOrDefault();
        if (apiKey != "masiu-internal-key")
        {
            return Unauthorized();
        }

        var success = await _senderService.SendToTopicAsync(
            request.Topic,
            Enum.Parse<NotificationType>(request.Type),
            request.Title,
            request.Body,
            request.ImageUrl,
            request.Data,
            cancellationToken);

        return success ? Ok() : BadRequest(new { message = "Topic notification failed" });
    }

    /// <summary>
    /// Health check.
    /// </summary>
    [HttpGet("/health")]
    [AllowAnonymous]
    public IActionResult Health() => Ok(new { status = "healthy", service = "notification-service" });

    private static NotificationDto MapToDto(Notification n) => new(
        n.Id, n.Type.ToString(), n.Title, n.Body, n.ImageUrl,
        n.Data, n.Status.ToString(), n.CreatedAt, n.SentAt, n.ReadAt
    );
}

// DTOs
public record NotificationsResponse(
    List<NotificationDto> Notifications,
    int UnreadCount,
    int Page,
    int PageSize
);

public record NotificationDto(
    string Id,
    string Type,
    string Title,
    string Body,
    string? ImageUrl,
    Dictionary<string, string> Data,
    string Status,
    DateTime CreatedAt,
    DateTime? SentAt,
    DateTime? ReadAt
);

public record UnreadCountResponse(int Count);

public record SendNotificationRequest(
    string UserId,
    List<string> FcmTokens,
    string Type,
    string Title,
    string Body,
    string? ImageUrl,
    Dictionary<string, string>? Data,
    string? DeduplicationKey
);

public record SendTopicRequest(
    string Topic,
    string Type,
    string Title,
    string Body,
    string? ImageUrl,
    Dictionary<string, string>? Data
);
