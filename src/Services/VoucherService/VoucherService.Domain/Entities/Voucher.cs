namespace VoucherService.Domain.Entities;

/// <summary>
/// Voucher entity - Aggregate Root.
/// Represents a discount voucher/code from e-commerce platforms.
/// </summary>
public class Voucher
{
    public string Id { get; private set; } = GenerateObjectId();
    public string Code { get; private set; } = null!;
    public string? Description { get; private set; }
    public Platform Platform { get; private set; }
    public VoucherType Type { get; private set; }
    
    // Discount info
    public decimal? DiscountValue { get; private set; }
    public decimal? DiscountPercent { get; private set; }
    public decimal? MinSpend { get; private set; }
    public decimal? MaxDiscount { get; private set; }
    
    // Time tracking
    public DateTime StartAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    
    // Status
    public VoucherStatus Status { get; private set; } = VoucherStatus.Active;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    
    // Stats
    public int UsageCount { get; private set; }
    public int ViewCount { get; private set; }
    
    // Linked Post (optional - if voucher was shared via a post)
    public string? PostId { get; private set; }

    private Voucher() { }

    public static Voucher Create(
        string code,
        Platform platform,
        VoucherType type,
        DateTime startAt,
        DateTime expiresAt,
        string? description = null)
    {
        return new Voucher
        {
            Code = code,
            Platform = platform,
            Type = type,
            StartAt = startAt,
            ExpiresAt = expiresAt,
            Description = description
        };
    }

    public void SetDiscount(decimal? value, decimal? percent, decimal? minSpend, decimal? maxDiscount)
    {
        DiscountValue = value;
        DiscountPercent = percent;
        MinSpend = minSpend;
        MaxDiscount = maxDiscount;
        MarkUpdated();
    }

    public void LinkToPost(string postId)
    {
        PostId = postId;
        MarkUpdated();
    }

    public void IncrementUsage()
    {
        UsageCount++;
        MarkUpdated();
    }

    public void IncrementView()
    {
        ViewCount++;
    }

    public void Deactivate()
    {
        Status = VoucherStatus.Inactive;
        MarkUpdated();
    }

    public void MarkAsExpired()
    {
        Status = VoucherStatus.Expired;
        MarkUpdated();
    }

    /// <summary>
    /// Calculate remaining percentage based on time decay.
    /// Green (>=50%), Yellow (20-49%), Red (<20%)
    /// </summary>
    public int GetRemainingPercent()
    {
        var now = DateTime.UtcNow;
        
        if (now >= ExpiresAt) return 0;
        if (now <= StartAt) return 100;

        var totalDuration = (ExpiresAt - StartAt).TotalSeconds;
        var timeRemaining = (ExpiresAt - now).TotalSeconds;

        if (totalDuration <= 0) return 0;

        return (int)((timeRemaining / totalDuration) * 100);
    }

    public VoucherUrgency GetUrgency()
    {
        var remaining = GetRemainingPercent();
        return remaining switch
        {
            >= 50 => VoucherUrgency.Green,
            >= 20 => VoucherUrgency.Yellow,
            _ => VoucherUrgency.Red
        };
    }

    private void MarkUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    private static string GenerateObjectId()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var random = new Random();
        return $"{timestamp:x8}{random.Next(0, 0xFFFFFF):x6}{random.Next(0, 0xFFFF):x4}{random.Next(0, 0xFFFFFF):x6}";
    }
}

public enum Platform
{
    Shopee = 1,
    Lazada = 2,
    TikTokShop = 3,
    Tiki = 4,
    Sendo = 5,
    Other = 99
}

public enum VoucherType
{
    Percentage = 1,     // Giảm X%
    FixedAmount = 2,    // Giảm X đồng
    FreeShip = 3,       // Miễn phí ship
    Combo = 4           // Combo deals
}

public enum VoucherStatus
{
    Active = 1,
    Inactive = 2,
    Expired = 3
}

public enum VoucherUrgency
{
    Green = 1,   // >=50% time remaining
    Yellow = 2,  // 20-49% time remaining
    Red = 3      // <20% time remaining - almost expired!
}
