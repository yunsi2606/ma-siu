using System.Text.RegularExpressions;
using AffiliateService.Domain.Entities;
using AffiliateService.Domain.Strategies;
using Microsoft.Extensions.Options;

namespace AffiliateService.Infrastructure.Platforms;

/// <summary>
/// Lazada affiliate platform implementation.
/// </summary>
public class LazadaPlatform : IAffiliatePlatform
{
    private readonly LazadaAffiliateOptions _options;
    private static readonly Regex ProductIdPattern = new(@"-i(\d+)-s(\d+)", RegexOptions.Compiled);
    private static readonly Regex SkuPattern = new(@"sku=(\d+)", RegexOptions.Compiled);

    public Platform Platform => Platform.Lazada;

    public LazadaPlatform(IOptions<LazadaAffiliateOptions> options)
    {
        _options = options.Value;
    }

    public bool CanHandle(string url)
    {
        return url.Contains("lazada.vn") || url.Contains("lazada.com");
    }

    public string? ExtractProductId(string url)
    {
        // Format: https://www.lazada.vn/products/product-name-i123456789-s123456789.html
        var match = ProductIdPattern.Match(url);
        if (match.Success && match.Groups.Count >= 3)
        {
            return $"{match.Groups[1].Value}-{match.Groups[2].Value}";
        }

        // Alt format with SKU
        var skuMatch = SkuPattern.Match(url);
        if (skuMatch.Success)
        {
            return skuMatch.Groups[1].Value;
        }

        return null;
    }

    public async Task<AffiliateLinkResult> GenerateAffiliateLinkAsync(string originalUrl, CancellationToken cancellationToken = default)
    {
        var productId = ExtractProductId(originalUrl);
        
        // Add Lazada affiliate tracking params
        var affiliateUrl = AddAffiliateParams(originalUrl);
        var deepLink = !string.IsNullOrEmpty(productId) ? GetDeepLink(productId) : null;

        await Task.CompletedTask;
        return new AffiliateLinkResult(true, affiliateUrl, deepLink);
    }

    public string GetDeepLink(string productId)
    {
        var parts = productId.Split('-');
        if (parts.Length >= 1)
        {
            // Lazada deep link format
            return $"lazada://product/{parts[0]}";
        }
        return string.Empty;
    }

    private string AddAffiliateParams(string url)
    {
        var separator = url.Contains('?') ? '&' : '?';
        return $"{url}{separator}aff_id={_options.AffiliateId}&utm_source=masiu&utm_medium=affiliate";
    }
}

public class LazadaAffiliateOptions
{
    public const string SectionName = "Affiliate:Lazada";
    public string AffiliateId { get; set; } = string.Empty;
    public string AppKey { get; set; } = string.Empty;
    public string AppSecret { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.lazada.vn/rest";
}
