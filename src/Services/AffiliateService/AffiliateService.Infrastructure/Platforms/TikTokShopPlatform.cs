using System.Text.RegularExpressions;
using AffiliateService.Domain.Entities;
using AffiliateService.Domain.Strategies;
using Microsoft.Extensions.Options;

namespace AffiliateService.Infrastructure.Platforms;

/// <summary>
/// TikTok Shop affiliate platform implementation.
/// </summary>
public class TikTokShopPlatform : IAffiliatePlatform
{
    private readonly TikTokAffiliateOptions _options;
    private static readonly Regex ProductIdPattern = new(@"product/(\d+)", RegexOptions.Compiled);

    public Platform Platform => Platform.TikTokShop;

    public TikTokShopPlatform(IOptions<TikTokAffiliateOptions> options)
    {
        _options = options.Value;
    }

    public bool CanHandle(string url)
    {
        return url.Contains("tiktok.com/") || url.Contains("tiktokshop.");
    }

    public string? ExtractProductId(string url)
    {
        // Format: https://www.tiktok.com/@seller/product/1234567890
        var match = ProductIdPattern.Match(url);
        if (match.Success && match.Groups.Count >= 2)
        {
            return match.Groups[1].Value;
        }
        return null;
    }

    public async Task<AffiliateLinkResult> GenerateAffiliateLinkAsync(string originalUrl, CancellationToken cancellationToken = default)
    {
        var productId = ExtractProductId(originalUrl);
        
        var affiliateUrl = AddAffiliateParams(originalUrl);
        var deepLink = !string.IsNullOrEmpty(productId) ? GetDeepLink(productId) : null;

        await Task.CompletedTask;
        return new AffiliateLinkResult(true, affiliateUrl, deepLink);
    }

    public string GetDeepLink(string productId)
    {
        // TikTok deep link format
        return $"snssdk1128://product/detail?product_id={productId}";
    }

    private string AddAffiliateParams(string url)
    {
        var separator = url.Contains('?') ? '&' : '?';
        return $"{url}{separator}affiliate_id={_options.AffiliateId}&utm_source=masiu";
    }
}

public class TikTokAffiliateOptions
{
    public const string SectionName = "Affiliate:TikTok";
    public string AffiliateId { get; set; } = string.Empty;
    public string AppId { get; set; } = string.Empty;
    public string AppSecret { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://open-api.affiliate.tiktokglobalshop.com";
}
