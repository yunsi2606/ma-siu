using EventContracts;
using MassTransit;
using RewardService.Application.Interfaces;
using RewardService.Domain.Entities;

namespace RewardService.Application.Services;

/// <summary>
/// Service for managing user points.
/// </summary>
public class PointsService(
    IPointsBalanceRepository balanceRepository,
    IPointsTransactionRepository transactionRepository,
    IPublishEndpoint publishEndpoint)
{
    /// <summary>
    /// Add points to user (from affiliate click, task completion, etc.)
    /// </summary>
    public async Task<PointsBalance> AddPointsAsync(
        string userId,
        int amount,
        string reason,
        string? referenceId = null,
        bool isPending = false,
        CancellationToken ct = default)
    {
        var balance = await balanceRepository.GetOrCreateAsync(userId, ct);
        
        // Create transaction record
        var transaction = PointsTransaction.CreateEarn(userId, amount, reason, referenceId, isPending);
        await transactionRepository.AddAsync(transaction, ct);
        
        // Update balance
        balance.AddPoints(amount, isPending);
        await balanceRepository.UpdateAsync(balance, ct);
        
        // Publish event
        await publishEndpoint.Publish(new PointsEarnedEvent
        {
            UserId = userId,
            Points = amount,
            Reason = reason,
            IsPending = isPending,
            EarnedAt = DateTime.UtcNow
        }, ct);
        
        return balance;
    }

    /// <summary>
    /// Spend points (for redemption).
    /// </summary>
    public async Task<bool> SpendPointsAsync(
        string userId,
        int amount,
        string reason,
        string? referenceId = null,
        CancellationToken ct = default)
    {
        var balance = await balanceRepository.GetOrCreateAsync(userId, ct);
        
        if (!balance.SpendPoints(amount))
            return false;
        
        var transaction = PointsTransaction.CreateSpend(userId, amount, reason, referenceId);
        await transactionRepository.AddAsync(transaction, ct);
        await balanceRepository.UpdateAsync(balance, ct);
        
        return true;
    }

    /// <summary>
    /// Get user's current balance.
    /// </summary>
    public async Task<PointsBalance> GetBalanceAsync(string userId, CancellationToken ct = default)
    {
        return await balanceRepository.GetOrCreateAsync(userId, ct);
    }

    /// <summary>
    /// Get user's transaction history.
    /// </summary>
    public async Task<IReadOnlyList<PointsTransaction>> GetTransactionsAsync(
        string userId,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var skip = (page - 1) * pageSize;
        return await transactionRepository.GetByUserIdAsync(userId, skip, pageSize, ct);
    }
}
