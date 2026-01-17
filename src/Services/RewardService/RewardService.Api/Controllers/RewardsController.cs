using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RewardService.Application.Services;
using RewardService.Domain.Entities;

namespace RewardService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RewardsController(RewardCatalogService catalogService) : ControllerBase
{
    /// <summary>
    /// Get all available rewards.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<RewardsResponse>> GetCatalog(CancellationToken ct)
    {
        var rewards = await catalogService.GetCatalogAsync(ct);
        return Ok(new RewardsResponse(rewards.Select(MapToDto).ToList()));
    }

    /// <summary>
    /// Redeem a reward.
    /// </summary>
    [HttpPost("{rewardId}/redeem")]
    public async Task<IActionResult> Redeem(string rewardId, CancellationToken ct)
    {
        var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await catalogService.RedeemAsync(userId, rewardId, ct);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { message = "Redemption created", redemptionId = result.Redemption!.Id });
    }

    /// <summary>
    /// Get user's redemption history.
    /// </summary>
    [HttpGet("redemptions")]
    public async Task<ActionResult<RedemptionsResponse>> GetRedemptions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var redemptions = await catalogService.GetUserRedemptionsAsync(userId, page, pageSize, ct);
        return Ok(new RedemptionsResponse(redemptions.Select(MapRedemptionToDto).ToList()));
    }

    /// <summary>
    /// Admin: Create new reward.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateReward([FromBody] CreateRewardRequest request, CancellationToken ct)
    {
        var reward = await catalogService.CreateRewardAsync(
            request.Name,
            request.Description,
            request.PointsCost,
            request.Type,
            request.Quantity,
            request.ImageUrl,
            ct);

        return Created($"/api/rewards/{reward.Id}", MapToDto(reward));
    }

    private static RewardDto MapToDto(Reward r) => new(
        r.Id, r.Name, r.Description, r.PointsCost, r.Type.ToString(),
        r.QuantityAvailable, r.ImageUrl, r.IsActive
    );

    private static RedemptionDto MapRedemptionToDto(Redemption r) => new(
        r.Id, r.RewardId, r.RewardName, r.PointsSpent, r.Status.ToString(),
        r.RedemptionCode, r.CreatedAt, r.ProcessedAt
    );
}

// DTOs
public record RewardsResponse(List<RewardDto> Rewards);
public record RewardDto(string Id, string Name, string Description, int PointsCost, string Type, int QuantityAvailable, string? ImageUrl, bool IsActive);
public record CreateRewardRequest(string Name, string Description, int PointsCost, RewardType Type, int Quantity = -1, string? ImageUrl = null);
public record RedemptionsResponse(List<RedemptionDto> Redemptions);
public record RedemptionDto(string Id, string RewardId, string RewardName, int PointsSpent, string Status, string? RedemptionCode, DateTime CreatedAt, DateTime? ProcessedAt);
