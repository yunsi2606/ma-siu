using MediatR;
using VoucherService.Application.Commands;
using VoucherService.Application.Interfaces;
using VoucherService.Domain.Entities;

namespace VoucherService.Application.Handlers;

public class CreateVoucherHandler : IRequestHandler<CreateVoucherCommand, CreateVoucherResult>
{
    private readonly IVoucherRepository _repository;

    public CreateVoucherHandler(IVoucherRepository repository) => _repository = repository;

    public async Task<CreateVoucherResult> Handle(CreateVoucherCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check for duplicates
            var existing = await _repository.GetByCodeAsync(request.Code, request.Platform, cancellationToken);
            if (existing != null)
                return new CreateVoucherResult(string.Empty, false, "Voucher code already exists for this platform");

            var voucher = Voucher.Create(
                request.Code,
                request.Platform,
                request.Type,
                request.StartAt,
                request.ExpiresAt,
                request.Description
            );

            voucher.SetDiscount(
                request.DiscountValue,
                request.DiscountPercent,
                request.MinSpend,
                request.MaxDiscount
            );

            if (!string.IsNullOrEmpty(request.PostId))
                voucher.LinkToPost(request.PostId);

            await _repository.AddAsync(voucher, cancellationToken);
            return new CreateVoucherResult(voucher.Id, true);
        }
        catch (Exception ex)
        {
            return new CreateVoucherResult(string.Empty, false, ex.Message);
        }
    }
}

public class UpdateVoucherHandler : IRequestHandler<UpdateVoucherCommand, bool>
{
    private readonly IVoucherRepository _repository;

    public UpdateVoucherHandler(IVoucherRepository repository) => _repository = repository;

    public async Task<bool> Handle(UpdateVoucherCommand request, CancellationToken cancellationToken)
    {
        var voucher = await _repository.GetByIdAsync(request.VoucherId, cancellationToken);
        if (voucher == null) return false;

        voucher.SetDiscount(
            request.DiscountValue ?? voucher.DiscountValue,
            request.DiscountPercent ?? voucher.DiscountPercent,
            request.MinSpend ?? voucher.MinSpend,
            request.MaxDiscount ?? voucher.MaxDiscount
        );

        await _repository.UpdateAsync(voucher, cancellationToken);
        return true;
    }
}

public class DeactivateVoucherHandler : IRequestHandler<DeactivateVoucherCommand, bool>
{
    private readonly IVoucherRepository _repository;

    public DeactivateVoucherHandler(IVoucherRepository repository) => _repository = repository;

    public async Task<bool> Handle(DeactivateVoucherCommand request, CancellationToken cancellationToken)
    {
        var voucher = await _repository.GetByIdAsync(request.VoucherId, cancellationToken);
        if (voucher == null) return false;

        voucher.Deactivate();
        await _repository.UpdateAsync(voucher, cancellationToken);
        return true;
    }
}

public class LinkVoucherToPostHandler : IRequestHandler<LinkVoucherToPostCommand, bool>
{
    private readonly IVoucherRepository _repository;

    public LinkVoucherToPostHandler(IVoucherRepository repository) => _repository = repository;

    public async Task<bool> Handle(LinkVoucherToPostCommand request, CancellationToken cancellationToken)
    {
        var voucher = await _repository.GetByIdAsync(request.VoucherId, cancellationToken);
        if (voucher == null) return false;

        voucher.LinkToPost(request.PostId);
        await _repository.UpdateAsync(voucher, cancellationToken);
        return true;
    }
}

public class TrackVoucherUsageHandler : IRequestHandler<TrackVoucherUsageCommand, bool>
{
    private readonly IVoucherRepository _repository;

    public TrackVoucherUsageHandler(IVoucherRepository repository) => _repository = repository;

    public async Task<bool> Handle(TrackVoucherUsageCommand request, CancellationToken cancellationToken)
    {
        var voucher = await _repository.GetByIdAsync(request.VoucherId, cancellationToken);
        if (voucher == null) return false;

        voucher.IncrementUsage();
        await _repository.UpdateAsync(voucher, cancellationToken);
        return true;
    }
}

/// <summary>
/// Expires all vouchers past their expiry date.
/// Used by JobOrchestrator/Quartz background job.
/// </summary>
public class ExpireVouchersHandler : IRequestHandler<ExpireVouchersCommand, int>
{
    private readonly IVoucherRepository _repository;

    public ExpireVouchersHandler(IVoucherRepository repository) => _repository = repository;

    public async Task<int> Handle(ExpireVouchersCommand request, CancellationToken cancellationToken)
    {
        var expiredVouchers = await _repository.GetExpiringAsync(DateTime.UtcNow, cancellationToken);
        var count = 0;

        foreach (var voucher in expiredVouchers.Where(v => v.IsExpired && v.Status == VoucherStatus.Active))
        {
            voucher.MarkAsExpired();
            await _repository.UpdateAsync(voucher, cancellationToken);
            count++;
        }

        return count;
    }
}
