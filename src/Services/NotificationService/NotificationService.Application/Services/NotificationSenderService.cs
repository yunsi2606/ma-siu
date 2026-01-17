using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Domain.Templates;

namespace NotificationService.Application.Services;

/// <summary>
/// Notification sending service.
/// Handles FCM push notifications with deduplication.
/// </summary>
public class NotificationSenderService
{
    private readonly IFcmService _fcmService;
    private readonly INotificationRepository _repository;
    private readonly IDeduplicationService _deduplication;

    public NotificationSenderService(
        IFcmService fcmService,
        INotificationRepository repository,
        IDeduplicationService deduplication)
    {
        _fcmService = fcmService;
        _repository = repository;
        _deduplication = deduplication;
    }

    /// <summary>
    /// Send notification to user with deduplication.
    /// </summary>
    public async Task<bool> SendAsync(
        string userId,
        IReadOnlyList<string> fcmTokens,
        NotificationType type,
        string title,
        string body,
        string? imageUrl = null,
        Dictionary<string, string>? data = null,
        string? deduplicationKey = null,
        CancellationToken cancellationToken = default)
    {
        // Deduplication check
        if (!string.IsNullOrEmpty(deduplicationKey))
        {
            var isDuplicate = await _deduplication.IsDuplicateAsync(userId, deduplicationKey, cancellationToken);
            if (isDuplicate) return false;
        }

        // Create notification record
        var notification = Notification.Create(userId, type, title, body, imageUrl, data);
        await _repository.AddAsync(notification, cancellationToken);

        if (fcmTokens.Count == 0) return true;

        // Send via FCM
        if (fcmTokens.Count == 1)
        {
            var result = await _fcmService.SendAsync(
                fcmTokens[0], title, body, imageUrl, data, cancellationToken);
            
            if (result.Success)
                notification.MarkSent(result.MessageId);
            else
                notification.MarkFailed(result.Error ?? "Unknown error");
        }
        else
        {
            var result = await _fcmService.SendMulticastAsync(
                fcmTokens, title, body, imageUrl, data, cancellationToken);
            
            notification.MarkSent($"Batch: {result.SuccessCount}/{fcmTokens.Count} success");
        }

        await _repository.UpdateAsync(notification, cancellationToken);

        // Mark as sent for deduplication
        if (!string.IsNullOrEmpty(deduplicationKey))
        {
            await _deduplication.MarkSentAsync(userId, deduplicationKey, TimeSpan.FromHours(24), cancellationToken);
        }

        return true;
    }

    /// <summary>
    /// Send new voucher notification using template.
    /// </summary>
    public async Task<bool> SendNewVoucherAsync(
        string userId,
        IReadOnlyList<string> fcmTokens,
        string voucherCode,
        string platform,
        string? discountInfo = null,
        string? postId = null,
        CancellationToken cancellationToken = default)
    {
        var (title, body, data) = NotificationTemplates.NewVoucher(voucherCode, platform, discountInfo, postId);
        return await SendAsync(userId, fcmTokens, NotificationType.NewVoucher, title, body, null, data, 
            $"voucher:{voucherCode}", cancellationToken);
    }

    /// <summary>
    /// Send voucher expiring notification using template.
    /// </summary>
    public async Task<bool> SendVoucherExpiringAsync(
        string userId,
        IReadOnlyList<string> fcmTokens,
        string voucherCode,
        string platform,
        int hoursRemaining,
        string? postId = null,
        CancellationToken cancellationToken = default)
    {
        var (title, body, data) = NotificationTemplates.VoucherExpiring(voucherCode, platform, hoursRemaining, postId);
        return await SendAsync(userId, fcmTokens, NotificationType.VoucherExpiring, title, body, null, data,
            $"expiring:{voucherCode}", cancellationToken);
    }

    /// <summary>
    /// Send to topic (broadcast).
    /// </summary>
    public async Task<bool> SendToTopicAsync(
        string topic,
        NotificationType type,
        string title,
        string body,
        string? imageUrl = null,
        Dictionary<string, string>? data = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _fcmService.SendToTopicAsync(topic, title, body, imageUrl, data, cancellationToken);
        return result.Success;
    }
}
