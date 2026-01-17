namespace NotificationService.Application.Interfaces;

/// <summary>
/// Firebase Cloud Messaging service interface.
/// </summary>
public interface IFcmService
{
    /// <summary>
    /// Send push notification to a single device.
    /// </summary>
    Task<FcmResult> SendAsync(string token, string title, string body, string? imageUrl = null, Dictionary<string, string>? data = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send push notification to multiple devices.
    /// </summary>
    Task<FcmBatchResult> SendMulticastAsync(IReadOnlyList<string> tokens, string title, string body, string? imageUrl = null, Dictionary<string, string>? data = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send to topic subscribers.
    /// </summary>
    Task<FcmResult> SendToTopicAsync(string topic, string title, string body, string? imageUrl = null, Dictionary<string, string>? data = null, CancellationToken cancellationToken = default);
}

public record FcmResult(bool Success, string? MessageId = null, string? Error = null);

public record FcmBatchResult(int SuccessCount, int FailureCount, IReadOnlyList<FcmResult> Results);
