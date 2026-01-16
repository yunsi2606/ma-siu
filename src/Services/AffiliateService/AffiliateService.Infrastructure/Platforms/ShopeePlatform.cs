using System.Text.RegularExpressions;
using AffiliateService.Domain.Entities;
using AffiliateService.Domain.Strategies;
using Microsoft.Extensions.Options;

namespace AffiliateService.Infrastructure.Platforms;

/// <summary>
/// Shopee affiliate platform implementation.
/// Handles URL detection, product extraction, and affiliate link generation.
/// </summary>
public class ShopeePlatform : IAffiliatePlatform
{
    private readonly ShopeeAffiliateOptions _options;
    private static readonly Regex ProductIdPattern = new(@"i\.(\d+)\.(\d+)", RegexOptions.Compiled);
    private static readonly Regex ShortLinkPattern = new(@"shopee\.vn/[^/]+$", RegexOptions.Compiled);

    public Platform Platform => Platform.Shopee;

    public ShopeePlatform(IOptions<ShopeeAffiliateOptions> options)
    {
        _options = options.Value;
    }

    public bool CanHandle(string url)
    {
        return url.Contains("shopee.vn") || url.Contains("shopee.co");
    }

    public string? ExtractProductId(string url)
    {
        // Format: https://shopee.vn/product.i.shopId.itemId
        var match = ProductIdPattern.Match(url);
        if (match.Success && match.Groups.Count >= 3)
        {
            return $"{match.Groups[1].Value}.{match.Groups[2].Value}";
        }
        return null;
    }

    public async Task<AffiliateLinkResult> GenerateAffiliateLinkAsync(string originalUrl, CancellationToken cancellationToken = default)
    {
        // Shopee affiliate link format (Deep Link approach)
        // Since we don't have official API access, we use deep link approach
        var productId = ExtractProductId(originalUrl);
        
        if (string.IsNullOrEmpty(productId))
        {
            // For non-product links, add affiliate parameters
            var affiliateUrl = AddAffiliateParams(originalUrl);
            return new AffiliateLinkResult(true, affiliateUrl, null);
        }

        var parts = productId.Split('.');
        if (parts.Length != 2)
        {
            return new AffiliateLinkResult(false, null, null, "Invalid product ID format");
        }

        var shopId = parts[0];
        var itemId = parts[1];

        // Generate tracked affiliate URL
        var affiliateLink = $"https://shopee.vn/product/{shopId}/{itemId}?{GetAffiliateQueryString()}";
        var deepLink = GetDeepLink(productId);

        await Task.CompletedTask; // Async placeholder for future API calls
        return new AffiliateLinkResult(true, affiliateLink, deepLink);
    }

    public string GetDeepLink(string productId)
    {
        var parts = productId.Split('.');
        if (parts.Length != 2) return string.Empty;

        // Shopee deep link format
        return $"shopeevn://product/{parts[0]}/{parts[1]}";
    }

    private string AddAffiliateParams(string url)
    {
        var separator = url.Contains('?') ? '&' : '?';
        return $"{url}{separator}{GetAffiliateQueryString()}";
    }

    private string GetAffiliateQueryString()
    {
        return $"af_id={_options.AffiliateId}&utm_source=masiu&utm_medium=affiliate";
    }
}

public class ShopeeAffiliateOptions
{
    public const string SectionName = "Affiliate:Shopee";
    public string AffiliateId { get; set; } = string.Empty;
    public string AppId { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://open-api.affiliate.shopee.vn";
}
