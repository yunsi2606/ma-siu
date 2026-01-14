namespace PostService.Domain.Entities;

/// <summary>
/// Post entity - Aggregate Root for deal posts.
/// Full DDD with domain events.
/// </summary>
public class Post
{
    public string Id { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public string AuthorId { get; private set; } = null!;
    public Platform Platform { get; private set; }
    public string OriginalUrl { get; private set; } = null!;
    public string? AffiliateUrl { get; private set; }
    public decimal? OriginalPrice { get; private set; }
    public decimal? SalePrice { get; private set; }
    public int? DiscountPercent { get; private set; }
    public DateTime? VoucherExpiresAt { get; private set; }
    public PostStatus Status { get; private set; } = PostStatus.Draft;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    
    // Media (images stored in MinIO)
    private readonly List<PostMedia> _media = new();
    public IReadOnlyCollection<PostMedia> Media => _media.AsReadOnly();
    
    // Stats
    public int ViewCount { get; private set; }
    public int ClickCount { get; private set; }

    private Post() { }

    public static Post Create(
        string title,
        string authorId,
        Platform platform,
        string originalUrl,
        string? description = null)
    {
        return new Post
        {
            Id = Guid.NewGuid().ToString(),
            Title = title,
            Description = description,
            AuthorId = authorId,
            Platform = platform,
            OriginalUrl = originalUrl,
            Status = PostStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string title, string? description, decimal? originalPrice, decimal? salePrice, DateTime? voucherExpiresAt)
    {
        Title = title;
        Description = description;
        OriginalPrice = originalPrice;
        SalePrice = salePrice;
        VoucherExpiresAt = voucherExpiresAt;
        
        if (originalPrice.HasValue && salePrice.HasValue && originalPrice > 0)
        {
            DiscountPercent = (int)((originalPrice.Value - salePrice.Value) / originalPrice.Value * 100);
        }
        
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAffiliateUrl(string affiliateUrl)
    {
        AffiliateUrl = affiliateUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddMedia(string objectKey, MediaType type, int order = 0)
    {
        _media.Add(new PostMedia(objectKey, type, order));
        UpdatedAt = DateTime.UtcNow;
    }

    public void ClearMedia()
    {
        _media.Clear();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Publish()
    {
        if (Status == PostStatus.Published)
            return;
            
        Status = PostStatus.Published;
        PublishedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        
        // Domain event would be raised here in full DDD
        // AddDomainEvent(new PostPublishedEvent(Id));
    }

    public void Unpublish()
    {
        Status = PostStatus.Draft;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementViewCount()
    {
        ViewCount++;
    }

    public void IncrementClickCount()
    {
        ClickCount++;
    }
}

public enum Platform
{
    Shopee = 1,
    Lazada = 2,
    TikTokShop = 3,
    Other = 99
}

public enum PostStatus
{
    Draft = 0,
    Published = 1,
    Archived = 2
}

public enum MediaType
{
    Image = 1,
    Video = 2
}

public record PostMedia(
    string ObjectKey,
    MediaType Type,
    int Order = 0
);
