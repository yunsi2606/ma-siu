using AffiliateService.Domain.Entities;

namespace AffiliateService.Domain.Strategies;

/// <summary>
/// Strategy Pattern interface for affiliate platform handling.
/// Each platform (Shopee, Lazada, TikTok) provides its own implementation.
/// </summary>
public interface IAffiliatePlatform
{
    /// <summary>
    /// Platform this strategy handles.
    /// </summary>
    Platform Platform { get; }

    /// <summary>
    /// Check if URL belongs to this platform.
    /// </summary>
    bool CanHandle(string url);

    /// <summary>
    /// Extract product ID from platform URL.
    /// </summary>
    string? ExtractProductId(string url);

    /// <summary>
    /// Generate affiliate link from original URL.
    /// </summary>
    Task<AffiliateLinkResult> GenerateAffiliateLinkAsync(string originalUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get deep link for mobile app.
    /// </summary>
    string GetDeepLink(string productId);
}

/// <summary>
/// Result of generating an affiliate link.
/// </summary>
public record AffiliateLinkResult(
    bool Success,
    string? AffiliateUrl,
    string? DeepLink,
    string? Error = null
);
