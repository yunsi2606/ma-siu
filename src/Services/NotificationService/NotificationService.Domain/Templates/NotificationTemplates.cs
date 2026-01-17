using NotificationService.Domain.Entities;

namespace NotificationService.Domain.Templates;

/// <summary>
/// Notification template factory.
/// Creates pre-defined notification content based on type and parameters.
/// </summary>
public static class NotificationTemplates
{
    public static (string Title, string Body, Dictionary<string, string> Data) NewVoucher(
        string voucherCode,
        string platform,
        string? discountInfo,
        string? postId = null)
    {
        return (
            $"🎁 Mã giảm giá mới từ {platform}!",
            $"Code: {voucherCode}" + (discountInfo != null ? $" - {discountInfo}" : ""),
            new Dictionary<string, string>
            {
                ["type"] = NotificationType.NewVoucher.ToString(),
                ["voucherCode"] = voucherCode,
                ["platform"] = platform,
                ["postId"] = postId ?? ""
            }
        );
    }

    public static (string Title, string Body, Dictionary<string, string> Data) VoucherExpiring(
        string voucherCode,
        string platform,
        int hoursRemaining,
        string? postId = null)
    {
        var urgency = hoursRemaining <= 2 ? "🔴" : hoursRemaining <= 6 ? "🟡" : "🟢";
        return (
            $"{urgency} Voucher sắp hết hạn!",
            $"Code {voucherCode} ({platform}) sẽ hết hạn trong {hoursRemaining} giờ nữa!",
            new Dictionary<string, string>
            {
                ["type"] = NotificationType.VoucherExpiring.ToString(),
                ["voucherCode"] = voucherCode,
                ["platform"] = platform,
                ["hoursRemaining"] = hoursRemaining.ToString(),
                ["postId"] = postId ?? ""
            }
        );
    }

    public static (string Title, string Body, Dictionary<string, string> Data) NewPost(
        string authorName,
        string postTitle,
        string postId)
    {
        return (
            $"📱 {authorName} vừa đăng bài mới",
            postTitle.Length > 50 ? postTitle[..47] + "..." : postTitle,
            new Dictionary<string, string>
            {
                ["type"] = NotificationType.NewPost.ToString(),
                ["authorName"] = authorName,
                ["postId"] = postId
            }
        );
    }

    public static (string Title, string Body, Dictionary<string, string> Data) PointsEarned(
        int points,
        string reason)
    {
        return (
            $"🎉 +{points} điểm thưởng!",
            reason,
            new Dictionary<string, string>
            {
                ["type"] = NotificationType.PointsEarned.ToString(),
                ["points"] = points.ToString()
            }
        );
    }

    public static (string Title, string Body, Dictionary<string, string> Data) System(
        string title,
        string body)
    {
        return (
            $"📢 {title}",
            body,
            new Dictionary<string, string>
            {
                ["type"] = NotificationType.System.ToString()
            }
        );
    }
}
