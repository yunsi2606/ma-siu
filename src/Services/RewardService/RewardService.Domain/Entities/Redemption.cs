namespace RewardService.Domain.Entities;

/// <summary>
/// Records a reward redemption by user.
/// </summary>
public class Redemption
{
    public string Id { get; private set; } = null!;
    public string UserId { get; private set; } = null!;
    public string RewardId { get; private set; } = null!;
    public string RewardName { get; private set; } = null!;
    public int PointsSpent { get; private set; }
    public RedemptionStatus Status { get; private set; }
    public string? RedemptionCode { get; private set; } // For voucher type rewards
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    private Redemption() { }

    public static Redemption Create(
        string userId,
        Reward reward)
    {
        return new Redemption
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            RewardId = reward.Id,
            RewardName = reward.Name,
            PointsSpent = reward.PointsCost,
            Status = RedemptionStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Approve(string? redemptionCode = null, string? notes = null)
    {
        Status = RedemptionStatus.Approved;
        RedemptionCode = redemptionCode;
        Notes = notes;
        ProcessedAt = DateTime.UtcNow;
    }

    public void Reject(string? notes = null)
    {
        Status = RedemptionStatus.Rejected;
        Notes = notes;
        ProcessedAt = DateTime.UtcNow;
    }

    public void Complete(string? notes = null)
    {
        Status = RedemptionStatus.Completed;
        Notes = notes;
        ProcessedAt = DateTime.UtcNow;
    }
}

public enum RedemptionStatus
{
    Pending,
    Approved,
    Rejected,
    Completed
}
