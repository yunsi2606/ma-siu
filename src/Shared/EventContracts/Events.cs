namespace EventContracts;

/// <summary>
/// Published when a new post is created by admin.
/// Triggers push notification to all users.
/// </summary>
public record PostCreatedEvent
{
    public required string PostId { get; init; }
    public required string Title { get; init; }
    public required string Platform { get; init; }
    public required string? ImageUrl { get; init; }
    public required DateTime CreatedAt { get; init; }
}

/// <summary>
/// Published when a voucher is about to expire.
/// Used by Quartz job to send reminders.
/// </summary>
public record VoucherExpiringEvent
{
    public required string VoucherId { get; init; }
    public required string PostId { get; init; }
    public required string Title { get; init; }
    public required int RemainingPercent { get; init; }
    public required DateTime ExpiresAt { get; init; }
}

/// <summary>
/// Published when a user completes a task.
/// </summary>
public record TaskCompletedEvent
{
    public required string TaskId { get; init; }
    public required string UserId { get; init; }
    public required string TaskType { get; init; }
    public required int PointsEarned { get; init; }
    public required DateTime CompletedAt { get; init; }
}

/// <summary>
/// Published when points are earned (pending or approved).
/// </summary>
public record PointsEarnedEvent
{
    public required string UserId { get; init; }
    public required int Points { get; init; }
    public required string Reason { get; init; }
    public required bool IsPending { get; init; }
    public required DateTime EarnedAt { get; init; }
}

/// <summary>
/// Published when an affiliate link is clicked.
/// </summary>
public record AffiliateLinkClickedEvent
{
    public required string PostId { get; init; }
    public required string UserId { get; init; }
    public required string Platform { get; init; }
    public required string AffiliateUrl { get; init; }
    public required DateTime ClickedAt { get; init; }
}

/// <summary>
/// Published when a user signs in or registers.
/// </summary>
public record UserAuthenticatedEvent
{
    public required string UserId { get; init; }
    public required string Email { get; init; }
    public required bool IsNewUser { get; init; }
    public required DateTime AuthenticatedAt { get; init; }
}
