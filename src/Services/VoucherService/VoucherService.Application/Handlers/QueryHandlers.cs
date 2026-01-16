using MediatR;
using VoucherService.Application.Interfaces;
using VoucherService.Application.Queries;
using VoucherService.Domain.Entities;

namespace VoucherService.Application.Handlers;

public class GetVoucherByIdHandler : IRequestHandler<GetVoucherByIdQuery, VoucherDto?>
{
    private readonly IVoucherRepository _repository;

    public GetVoucherByIdHandler(IVoucherRepository repository) => _repository = repository;

    public async Task<VoucherDto?> Handle(GetVoucherByIdQuery request, CancellationToken cancellationToken)
    {
        var voucher = await _repository.GetByIdAsync(request.VoucherId, cancellationToken);
        if (voucher == null) return null;

        voucher.IncrementView();
        await _repository.UpdateAsync(voucher, cancellationToken);

        return MapToDto(voucher);
    }

    private static VoucherDto MapToDto(Voucher v) => new(
        v.Id, v.Code, v.Description,
        v.Platform.ToString(), v.Type.ToString(),
        v.DiscountValue, v.DiscountPercent, v.MinSpend, v.MaxDiscount,
        v.StartAt, v.ExpiresAt,
        v.GetRemainingPercent(), v.GetUrgency().ToString(),
        v.Status.ToString(), v.UsageCount, v.ViewCount, v.PostId
    );
}

public class GetActiveVouchersHandler : IRequestHandler<GetActiveVouchersQuery, VouchersResult>
{
    private readonly IVoucherRepository _repository;

    public GetActiveVouchersHandler(IVoucherRepository repository) => _repository = repository;

    public async Task<VouchersResult> Handle(GetActiveVouchersQuery request, CancellationToken cancellationToken)
    {
        var skip = (request.Page - 1) * request.PageSize;
        var vouchers = await _repository.GetActiveAsync(skip, request.PageSize, request.Platform, cancellationToken);
        var totalCount = await _repository.GetActiveCountAsync(request.Platform, cancellationToken);

        var dtos = vouchers.Select(MapToDto).ToList();
        return new VouchersResult(dtos, totalCount, request.Page, request.PageSize);
    }

    private static VoucherDto MapToDto(Voucher v) => new(
        v.Id, v.Code, v.Description,
        v.Platform.ToString(), v.Type.ToString(),
        v.DiscountValue, v.DiscountPercent, v.MinSpend, v.MaxDiscount,
        v.StartAt, v.ExpiresAt,
        v.GetRemainingPercent(), v.GetUrgency().ToString(),
        v.Status.ToString(), v.UsageCount, v.ViewCount, v.PostId
    );
}

public class GetExpiringSoonHandler : IRequestHandler<GetExpiringSoonQuery, IReadOnlyList<VoucherDto>>
{
    private readonly IVoucherRepository _repository;

    public GetExpiringSoonHandler(IVoucherRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<VoucherDto>> Handle(GetExpiringSoonQuery request, CancellationToken cancellationToken)
    {
        var threshold = DateTime.UtcNow.AddHours(request.HoursAhead);
        var vouchers = await _repository.GetExpiringAsync(threshold, cancellationToken);
        return vouchers.Where(v => v.Status == VoucherStatus.Active).Select(MapToDto).ToList();
    }

    private static VoucherDto MapToDto(Voucher v) => new(
        v.Id, v.Code, v.Description,
        v.Platform.ToString(), v.Type.ToString(),
        v.DiscountValue, v.DiscountPercent, v.MinSpend, v.MaxDiscount,
        v.StartAt, v.ExpiresAt,
        v.GetRemainingPercent(), v.GetUrgency().ToString(),
        v.Status.ToString(), v.UsageCount, v.ViewCount, v.PostId
    );
}

public class GetVouchersByPostHandler : IRequestHandler<GetVouchersByPostQuery, IReadOnlyList<VoucherDto>>
{
    private readonly IVoucherRepository _repository;

    public GetVouchersByPostHandler(IVoucherRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<VoucherDto>> Handle(GetVouchersByPostQuery request, CancellationToken cancellationToken)
    {
        var vouchers = await _repository.GetByPostIdAsync(request.PostId, cancellationToken);
        return vouchers.Select(MapToDto).ToList();
    }

    private static VoucherDto MapToDto(Voucher v) => new(
        v.Id, v.Code, v.Description,
        v.Platform.ToString(), v.Type.ToString(),
        v.DiscountValue, v.DiscountPercent, v.MinSpend, v.MaxDiscount,
        v.StartAt, v.ExpiresAt,
        v.GetRemainingPercent(), v.GetUrgency().ToString(),
        v.Status.ToString(), v.UsageCount, v.ViewCount, v.PostId
    );
}
