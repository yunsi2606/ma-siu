using RewardService.Domain.Entities;

namespace RewardService.Application.Interfaces;

public interface IRewardRepository
{
    Task<Reward?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<Reward>> GetAllActiveAsync(CancellationToken ct = default);
    Task AddAsync(Reward reward, CancellationToken ct = default);
    Task UpdateAsync(Reward reward, CancellationToken ct = default);
}

public interface IPointsBalanceRepository
{
    Task<PointsBalance?> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<PointsBalance> GetOrCreateAsync(string userId, CancellationToken ct = default);
    Task UpdateAsync(PointsBalance balance, CancellationToken ct = default);
}

public interface IPointsTransactionRepository
{
    Task<IReadOnlyList<PointsTransaction>> GetByUserIdAsync(string userId, int skip, int take, CancellationToken ct = default);
    Task<IReadOnlyList<PointsTransaction>> GetPendingAsync(CancellationToken ct = default);
    Task AddAsync(PointsTransaction transaction, CancellationToken ct = default);
    Task UpdateAsync(PointsTransaction transaction, CancellationToken ct = default);
}

public interface IRedemptionRepository
{
    Task<Redemption?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<Redemption>> GetByUserIdAsync(string userId, int skip, int take, CancellationToken ct = default);
    Task<IReadOnlyList<Redemption>> GetPendingAsync(CancellationToken ct = default);
    Task AddAsync(Redemption redemption, CancellationToken ct = default);
    Task UpdateAsync(Redemption redemption, CancellationToken ct = default);
}
