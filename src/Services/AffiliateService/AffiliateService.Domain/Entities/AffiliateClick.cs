namespace AffiliateService.Domain.Entities;

/// <summary>
/// Affiliate click tracking entity.
/// Records each click on an affiliate link.
/// </summary>
public class AffiliateClick
{
    public string Id { get; private set; } = GenerateObjectId();
    public string PostId { get; private set; } = null!;
    public string OriginalUrl { get; private set; } = null!;
    public string AffiliateUrl { get; private set; } = null!;
    public Platform Platform { get; private set; }
    
    // Tracking info
    public string? UserId { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? Referer { get; private set; }
    
    public DateTime ClickedAt { get; private set; } = DateTime.UtcNow;

    private AffiliateClick() { }

    public static AffiliateClick Create(
        string postId,
        string originalUrl,
        string affiliateUrl,
        Platform platform,
        string? userId = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? referer = null)
    {
        return new AffiliateClick
        {
            PostId = postId,
            OriginalUrl = originalUrl,
            AffiliateUrl = affiliateUrl,
            Platform = platform,
            UserId = userId,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Referer = referer
        };
    }

    private static string GenerateObjectId()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var random = new Random();
        return $"{timestamp:x8}{random.Next(0, 0xFFFFFF):x6}{random.Next(0, 0xFFFF):x4}{random.Next(0, 0xFFFFFF):x6}";
    }
}

/// <summary>
/// E-commerce platforms supported for affiliate links.
/// </summary>
public enum Platform
{
    Unknown = 0,
    Shopee = 1,
    Lazada = 2,
    TikTokShop = 3,
    Tiki = 4,
    Sendo = 5
}
