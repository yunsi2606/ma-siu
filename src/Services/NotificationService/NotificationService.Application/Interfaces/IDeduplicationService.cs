namespace NotificationService.Application.Interfaces;

/// <summary>
/// Deduplication service interface.
/// Prevents sending duplicate notifications.
/// </summary>
public interface IDeduplicationService
{
    /// <summary>
    /// Check if notification with this key was already sent.
    /// </summary>
    Task<bool> IsDuplicateAsync(string userId, string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mark notification as sent (for deduplication).
    /// </summary>
    Task MarkSentAsync(string userId, string key, TimeSpan ttl, CancellationToken cancellationToken = default);
}
