namespace RewardService.Domain.Entities;

/// <summary>
/// User's points balance.
/// </summary>
public class PointsBalance
{
    public string UserId { get; private set; } = null!;
    public int TotalPoints { get; private set; }
    public int AvailablePoints { get; private set; }
    public int PendingPoints { get; private set; }
    public int LifetimePoints { get; private set; }
    public DateTime LastUpdated { get; private set; }

    private PointsBalance() { }

    public static PointsBalance Create(string userId)
    {
        return new PointsBalance
        {
            UserId = userId,
            TotalPoints = 0,
            AvailablePoints = 0,
            PendingPoints = 0,
            LifetimePoints = 0,
            LastUpdated = DateTime.UtcNow
        };
    }

    public void AddPoints(int amount, bool isPending = false)
    {
        if (isPending)
        {
            PendingPoints += amount;
        }
        else
        {
            AvailablePoints += amount;
            TotalPoints += amount;
            LifetimePoints += amount;
        }
        LastUpdated = DateTime.UtcNow;
    }

    public void ApprovePendingPoints(int amount)
    {
        if (amount > PendingPoints) amount = PendingPoints;
        PendingPoints -= amount;
        AvailablePoints += amount;
        TotalPoints += amount;
        LifetimePoints += amount;
        LastUpdated = DateTime.UtcNow;
    }

    public void RejectPendingPoints(int amount)
    {
        if (amount > PendingPoints) amount = PendingPoints;
        PendingPoints -= amount;
        LastUpdated = DateTime.UtcNow;
    }

    public bool CanSpend(int amount)
    {
        return AvailablePoints >= amount;
    }

    public bool SpendPoints(int amount)
    {
        if (!CanSpend(amount)) return false;
        AvailablePoints -= amount;
        TotalPoints -= amount;
        LastUpdated = DateTime.UtcNow;
        return true;
    }
}
