namespace RewardService.Domain.Entities;

/// <summary>
/// Records a points transaction (earn or spend).
/// </summary>
public class PointsTransaction
{
    public string Id { get; private set; } = null!;
    public string UserId { get; private set; } = null!;
    public int Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public string Reason { get; private set; } = null!;
    public string? ReferenceId { get; private set; } // RewardId, TaskId, etc.
    public TransactionStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    private PointsTransaction() { }

    public static PointsTransaction CreateEarn(
        string userId,
        int amount,
        string reason,
        string? referenceId = null,
        bool isPending = false)
    {
        return new PointsTransaction
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Amount = amount,
            Type = TransactionType.Earn,
            Reason = reason,
            ReferenceId = referenceId,
            Status = isPending ? TransactionStatus.Pending : TransactionStatus.Completed,
            CreatedAt = DateTime.UtcNow,
            ProcessedAt = isPending ? null : DateTime.UtcNow
        };
    }

    public static PointsTransaction CreateSpend(
        string userId,
        int amount,
        string reason,
        string? referenceId = null)
    {
        return new PointsTransaction
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Amount = -amount, // Negative for spending
            Type = TransactionType.Spend,
            Reason = reason,
            ReferenceId = referenceId,
            Status = TransactionStatus.Completed,
            CreatedAt = DateTime.UtcNow,
            ProcessedAt = DateTime.UtcNow
        };
    }

    public void Approve()
    {
        Status = TransactionStatus.Completed;
        ProcessedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        Status = TransactionStatus.Rejected;
        ProcessedAt = DateTime.UtcNow;
    }
}

public enum TransactionType
{
    Earn,
    Spend
}

public enum TransactionStatus
{
    Pending,
    Completed,
    Rejected
}
