using AffiliateService.Application.Interfaces;
using AffiliateService.Application.Services;
using AffiliateService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AffiliateService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AffiliateController : ControllerBase
{
    private readonly AffiliateLinkService _linkService;
    private readonly IAffiliateClickRepository _clickRepository;
    private readonly ILogger<AffiliateController> _logger;

    public AffiliateController(
        AffiliateLinkService linkService,
        IAffiliateClickRepository clickRepository,
        ILogger<AffiliateController> logger)
    {
        _linkService = linkService;
        _clickRepository = clickRepository;
        _logger = logger;
    }

    /// <summary>
    /// Generate affiliate link from original URL.
    /// </summary>
    [HttpPost("generate")]
    [Authorize]
    public async Task<ActionResult<GenerateAffiliateResponse>> GenerateAffiliateLink(
        [FromBody] GenerateAffiliateRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _linkService.GenerateAffiliateLinkAsync(request.Url, cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new { error = result.Error });
        }

        var (platform, productId) = _linkService.ExtractProductInfo(request.Url);

        return Ok(new GenerateAffiliateResponse(
            request.Url,
            result.AffiliateUrl!,
            result.DeepLink,
            platform.ToString(),
            productId
        ));
    }

    /// <summary>
    /// Detect platform from URL.
    /// </summary>
    [HttpPost("detect")]
    public ActionResult<DetectPlatformResponse> DetectPlatform([FromBody] DetectPlatformRequest request)
    {
        var (platform, productId) = _linkService.ExtractProductInfo(request.Url);

        return Ok(new DetectPlatformResponse(
            platform.ToString(),
            platform != Platform.Unknown,
            productId
        ));
    }

    /// <summary>
    /// Track affiliate link click.
    /// Called by API Gateway /go/{postId} endpoint.
    /// </summary>
    [HttpPost("track")]
    public async Task<IActionResult> TrackClick([FromBody] TrackClickRequest request, CancellationToken cancellationToken)
    {
        var platform = _linkService.DetectPlatform(request.OriginalUrl);

        var click = AffiliateClick.Create(
            request.PostId,
            request.OriginalUrl,
            request.AffiliateUrl,
            platform,
            request.UserId,
            request.IpAddress,
            request.UserAgent,
            request.Referer
        );

        await _clickRepository.AddAsync(click, cancellationToken);

        _logger.LogInformation("Tracked click for post {PostId} on {Platform}", request.PostId, platform);
        return NoContent();
    }

    /// <summary>
    /// Get click stats for a post.
    /// </summary>
    [HttpGet("stats/{postId}")]
    public async Task<ActionResult<ClickStatsResponse>> GetClickStats(string postId, CancellationToken cancellationToken)
    {
        var count = await _clickRepository.GetClickCountByPostAsync(postId, cancellationToken);
        var recentClicks = await _clickRepository.GetByPostIdAsync(postId, 10, cancellationToken);

        return Ok(new ClickStatsResponse(
            postId,
            count,
            recentClicks.Select(c => new ClickInfo(c.Platform.ToString(), c.ClickedAt)).ToList()
        ));
    }

    /// <summary>
    /// Health check.
    /// </summary>
    [HttpGet("/health")]
    public IActionResult Health() => Ok(new { status = "healthy", service = "affiliate-service" });
}

// Request/Response DTOs
public record GenerateAffiliateRequest(string Url);
public record GenerateAffiliateResponse(string OriginalUrl, string AffiliateUrl, string? DeepLink, string Platform, string? ProductId);

public record DetectPlatformRequest(string Url);
public record DetectPlatformResponse(string Platform, bool Supported, string? ProductId);

public record TrackClickRequest(
    string PostId,
    string OriginalUrl,
    string AffiliateUrl,
    string? UserId,
    string? IpAddress,
    string? UserAgent,
    string? Referer
);

public record ClickStatsResponse(string PostId, int TotalClicks, List<ClickInfo> RecentClicks);
public record ClickInfo(string Platform, DateTime ClickedAt);
