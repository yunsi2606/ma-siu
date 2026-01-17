using RewardService.Application.Interfaces;
using RewardService.Domain.Entities;

namespace RewardService.Application.Services;

/// <summary>
/// Service for managing reward catalog and redemptions.
/// </summary>
public class RewardCatalogService(
    IRewardRepository rewardRepository,
    IRedemptionRepository redemptionRepository,
    PointsService pointsService)
{
    /// <summary>
    /// Get all active rewards.
    /// </summary>
    public async Task<IReadOnlyList<Reward>> GetCatalogAsync(CancellationToken ct = default)
    {
        return await rewardRepository.GetAllActiveAsync(ct);
    }

    /// <summary>
    /// Redeem a reward.
    /// </summary>
    public async Task<RedemptionResult> RedeemAsync(
        string userId,
        string rewardId,
        CancellationToken ct = default)
    {
        var reward = await rewardRepository.GetByIdAsync(rewardId, ct);
        
        if (reward == null)
            return RedemptionResult.Fail("Reward not found");
        
        if (!reward.CanRedeem())
            return RedemptionResult.Fail("Reward is not available");
        
        // Check user has enough points
        var balance = await pointsService.GetBalanceAsync(userId, ct);
        if (!balance.CanSpend(reward.PointsCost))
            return RedemptionResult.Fail("Insufficient points");
        
        // Spend points
        var spent = await pointsService.SpendPointsAsync(
            userId, reward.PointsCost, $"Redeem: {reward.Name}", rewardId, ct);
        
        if (!spent)
            return RedemptionResult.Fail("Failed to spend points");
        
        // Create redemption record
        var redemption = Redemption.Create(userId, reward);
        await redemptionRepository.AddAsync(redemption, ct);
        
        // Deduct reward quantity
        reward.DeductQuantity();
        await rewardRepository.UpdateAsync(reward, ct);
        
        return RedemptionResult.Success(redemption);
    }

    /// <summary>
    /// Get user's redemption history.
    /// </summary>
    public async Task<IReadOnlyList<Redemption>> GetUserRedemptionsAsync(
        string userId,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var skip = (page - 1) * pageSize;
        return await redemptionRepository.GetByUserIdAsync(userId, skip, pageSize, ct);
    }

    /// <summary>
    /// Admin: Create new reward.
    /// </summary>
    public async Task<Reward> CreateRewardAsync(
        string name,
        string description,
        int pointsCost,
        RewardType type,
        int quantity = -1,
        string? imageUrl = null,
        CancellationToken ct = default)
    {
        var reward = Reward.Create(name, description, pointsCost, type, quantity, imageUrl);
        await rewardRepository.AddAsync(reward, ct);
        return reward;
    }

    /// <summary>
    /// Admin: Approve redemption.
    /// </summary>
    public async Task<bool> ApproveRedemptionAsync(
        string redemptionId,
        string? redemptionCode = null,
        string? notes = null,
        CancellationToken ct = default)
    {
        var redemption = await redemptionRepository.GetByIdAsync(redemptionId, ct);
        if (redemption == null) return false;
        
        redemption.Approve(redemptionCode, notes);
        await redemptionRepository.UpdateAsync(redemption, ct);
        return true;
    }
}

public record RedemptionResult(bool IsSuccess, Redemption? Redemption, string? Error)
{
    public static RedemptionResult Success(Redemption redemption) => new(true, redemption, null);
    public static RedemptionResult Fail(string error) => new(false, null, error);
}
