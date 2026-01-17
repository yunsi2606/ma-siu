namespace NotificationService.Domain.Entities;

/// <summary>
/// Notification entity - tracks sent notifications.
/// </summary>
public class Notification
{
    public string Id { get; private set; } = GenerateObjectId();
    public string UserId { get; private set; } = null!;
    public NotificationType Type { get; private set; }
    public string Title { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public string? ImageUrl { get; private set; }
    public Dictionary<string, string> Data { get; private set; } = new();
    
    // Tracking
    public NotificationStatus Status { get; private set; } = NotificationStatus.Pending;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? SentAt { get; private set; }
    public DateTime? ReadAt { get; private set; }
    
    // FCM response
    public string? FcmMessageId { get; private set; }
    public string? ErrorMessage { get; private set; }

    private Notification() { }

    public static Notification Create(
        string userId,
        NotificationType type,
        string title,
        string body,
        string? imageUrl = null,
        Dictionary<string, string>? data = null)
    {
        return new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Body = body,
            ImageUrl = imageUrl,
            Data = data ?? new Dictionary<string, string>()
        };
    }

    public void MarkSent(string? fcmMessageId = null)
    {
        Status = NotificationStatus.Sent;
        SentAt = DateTime.UtcNow;
        FcmMessageId = fcmMessageId;
    }

    public void MarkFailed(string errorMessage)
    {
        Status = NotificationStatus.Failed;
        ErrorMessage = errorMessage;
    }

    public void MarkRead()
    {
        ReadAt = DateTime.UtcNow;
    }

    private static string GenerateObjectId()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var random = new Random();
        return $"{timestamp:x8}{random.Next(0, 0xFFFFFF):x6}{random.Next(0, 0xFFFF):x4}{random.Next(0, 0xFFFFFF):x6}";
    }
}

public enum NotificationType
{
    NewVoucher = 1,         // Có voucher mới từ shop yêu thích
    VoucherExpiring = 2,    // Voucher sắp hết hạn
    NewPost = 3,            // Có post mới từ người follow
    PointsEarned = 4,       // Nhận được điểm thưởng
    System = 5              // Thông báo hệ thống
}

public enum NotificationStatus
{
    Pending = 1,
    Sent = 2,
    Failed = 3
}
