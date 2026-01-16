using AffiliateService.Domain.Entities;
using AffiliateService.Domain.Strategies;

namespace AffiliateService.Application.Services;

/// <summary>
/// Affiliate Link Service - Facade for platform strategies.
/// Uses Strategy Pattern to handle different e-commerce platforms.
/// </summary>
public class AffiliateLinkService
{
    private readonly IEnumerable<IAffiliatePlatform> _platforms;

    public AffiliateLinkService(IEnumerable<IAffiliatePlatform> platforms)
    {
        _platforms = platforms;
    }

    /// <summary>
    /// Detect platform from URL.
    /// </summary>
    public Platform DetectPlatform(string url)
    {
        var platform = _platforms.FirstOrDefault(p => p.CanHandle(url));
        return platform?.Platform ?? Platform.Unknown;
    }

    /// <summary>
    /// Get strategy for specific platform.
    /// </summary>
    public IAffiliatePlatform? GetPlatformStrategy(Platform platform)
    {
        return _platforms.FirstOrDefault(p => p.Platform == platform);
    }

    /// <summary>
    /// Generate affiliate link for any supported platform.
    /// </summary>
    public async Task<AffiliateLinkResult> GenerateAffiliateLinkAsync(string url, CancellationToken cancellationToken = default)
    {
        var platform = _platforms.FirstOrDefault(p => p.CanHandle(url));
        if (platform == null)
        {
            return new AffiliateLinkResult(false, null, null, "Unsupported platform");
        }

        return await platform.GenerateAffiliateLinkAsync(url, cancellationToken);
    }

    /// <summary>
    /// Extract product ID from platform URL.
    /// </summary>
    public (Platform Platform, string? ProductId) ExtractProductInfo(string url)
    {
        var platform = _platforms.FirstOrDefault(p => p.CanHandle(url));
        if (platform == null)
        {
            return (Platform.Unknown, null);
        }

        return (platform.Platform, platform.ExtractProductId(url));
    }
}
