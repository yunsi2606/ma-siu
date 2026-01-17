namespace RewardService.Domain.Entities;

/// <summary>
/// Represents a redeemable reward in the catalog.
/// </summary>
public class Reward
{
    public string Id { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public int PointsCost { get; private set; }
    public string? ImageUrl { get; private set; }
    public RewardType Type { get; private set; }
    public int QuantityAvailable { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Reward() { }

    public static Reward Create(
        string name,
        string description,
        int pointsCost,
        RewardType type,
        int quantityAvailable = -1, // -1 = unlimited
        string? imageUrl = null)
    {
        return new Reward
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Description = description,
            PointsCost = pointsCost,
            Type = type,
            QuantityAvailable = quantityAvailable,
            ImageUrl = imageUrl,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public bool CanRedeem()
    {
        return IsActive && (QuantityAvailable == -1 || QuantityAvailable > 0);
    }

    public void DeductQuantity()
    {
        if (QuantityAvailable > 0)
        {
            QuantityAvailable--;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string description, int pointsCost, string? imageUrl)
    {
        Name = name;
        Description = description;
        PointsCost = pointsCost;
        ImageUrl = imageUrl;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum RewardType
{
    VoucherCode,      // Mã giảm giá
    PhysicalGift,     // Quà tặng vật lý
    DigitalGift,      // Gift card, topup
    ExclusiveAccess   // Quyền truy cập đặc biệt
}
