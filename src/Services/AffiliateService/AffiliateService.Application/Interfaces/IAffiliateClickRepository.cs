using AffiliateService.Domain.Entities;

namespace AffiliateService.Application.Interfaces;

public interface IAffiliateClickRepository
{
    Task<AffiliateClick?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AffiliateClick>> GetByPostIdAsync(string postId, int take = 100, CancellationToken cancellationToken = default);
    Task<int> GetClickCountByPostAsync(string postId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AffiliateClick>> GetByPlatformAsync(Platform platform, DateTime from, DateTime to, int take = 1000, CancellationToken cancellationToken = default);
    Task AddAsync(AffiliateClick click, CancellationToken cancellationToken = default);
}
