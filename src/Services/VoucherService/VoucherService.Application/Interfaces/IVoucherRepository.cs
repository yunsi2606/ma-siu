using VoucherService.Domain.Entities;

namespace VoucherService.Application.Interfaces;

public interface IVoucherRepository
{
    Task<Voucher?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Voucher?> GetByCodeAsync(string code, Platform platform, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Voucher>> GetActiveAsync(int skip, int take, Platform? platform = null, CancellationToken cancellationToken = default);
    Task<int> GetActiveCountAsync(Platform? platform = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Voucher>> GetExpiringAsync(DateTime threshold, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Voucher>> GetByPostIdAsync(string postId, CancellationToken cancellationToken = default);
    Task AddAsync(Voucher voucher, CancellationToken cancellationToken = default);
    Task UpdateAsync(Voucher voucher, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
