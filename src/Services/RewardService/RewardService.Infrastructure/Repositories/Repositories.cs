using MongoDB.Driver;
using RewardService.Application.Interfaces;
using RewardService.Domain.Entities;

namespace RewardService.Infrastructure.Repositories;

public class RewardRepository(IMongoDatabase database) : IRewardRepository
{
    private readonly IMongoCollection<Reward> _collection = database.GetCollection<Reward>("rewards");

    public async Task<Reward?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        return await _collection.Find(r => r.Id == id).FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<Reward>> GetAllActiveAsync(CancellationToken ct = default)
    {
        var result = await _collection.Find(r => r.IsActive).ToListAsync(ct);
        return result;
    }

    public async Task AddAsync(Reward reward, CancellationToken ct = default)
    {
        await _collection.InsertOneAsync(reward, cancellationToken: ct);
    }

    public async Task UpdateAsync(Reward reward, CancellationToken ct = default)
    {
        await _collection.ReplaceOneAsync(r => r.Id == reward.Id, reward, cancellationToken: ct);
    }
}

public class PointsBalanceRepository(IMongoDatabase database) : IPointsBalanceRepository
{
    private readonly IMongoCollection<PointsBalance> _collection = database.GetCollection<PointsBalance>("points_balances");

    public async Task<PointsBalance?> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        return await _collection.Find(p => p.UserId == userId).FirstOrDefaultAsync(ct);
    }

    public async Task<PointsBalance> GetOrCreateAsync(string userId, CancellationToken ct = default)
    {
        var balance = await GetByUserIdAsync(userId, ct);
        if (balance != null) return balance;

        balance = PointsBalance.Create(userId);
        await _collection.InsertOneAsync(balance, cancellationToken: ct);
        return balance;
    }

    public async Task UpdateAsync(PointsBalance balance, CancellationToken ct = default)
    {
        await _collection.ReplaceOneAsync(p => p.UserId == balance.UserId, balance, cancellationToken: ct);
    }
}

public class PointsTransactionRepository(IMongoDatabase database) : IPointsTransactionRepository
{
    private readonly IMongoCollection<PointsTransaction> _collection = database.GetCollection<PointsTransaction>("points_transactions");

    public async Task<IReadOnlyList<PointsTransaction>> GetByUserIdAsync(string userId, int skip, int take, CancellationToken ct = default)
    {
        var result = await _collection
            .Find(t => t.UserId == userId)
            .SortByDescending(t => t.CreatedAt)
            .Skip(skip)
            .Limit(take)
            .ToListAsync(ct);
        return result;
    }

    public async Task<IReadOnlyList<PointsTransaction>> GetPendingAsync(CancellationToken ct = default)
    {
        var result = await _collection.Find(t => t.Status == TransactionStatus.Pending).ToListAsync(ct);
        return result;
    }

    public async Task AddAsync(PointsTransaction transaction, CancellationToken ct = default)
    {
        await _collection.InsertOneAsync(transaction, cancellationToken: ct);
    }

    public async Task UpdateAsync(PointsTransaction transaction, CancellationToken ct = default)
    {
        await _collection.ReplaceOneAsync(t => t.Id == transaction.Id, transaction, cancellationToken: ct);
    }
}

public class RedemptionRepository(IMongoDatabase database) : IRedemptionRepository
{
    private readonly IMongoCollection<Redemption> _collection = database.GetCollection<Redemption>("redemptions");

    public async Task<Redemption?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        return await _collection.Find(r => r.Id == id).FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<Redemption>> GetByUserIdAsync(string userId, int skip, int take, CancellationToken ct = default)
    {
        var result = await _collection
            .Find(r => r.UserId == userId)
            .SortByDescending(r => r.CreatedAt)
            .Skip(skip)
            .Limit(take)
            .ToListAsync(ct);
        return result;
    }

    public async Task<IReadOnlyList<Redemption>> GetPendingAsync(CancellationToken ct = default)
    {
        var result = await _collection.Find(r => r.Status == RedemptionStatus.Pending).ToListAsync(ct);
        return result;
    }

    public async Task AddAsync(Redemption redemption, CancellationToken ct = default)
    {
        await _collection.InsertOneAsync(redemption, cancellationToken: ct);
    }

    public async Task UpdateAsync(Redemption redemption, CancellationToken ct = default)
    {
        await _collection.ReplaceOneAsync(r => r.Id == redemption.Id, redemption, cancellationToken: ct);
    }
}
