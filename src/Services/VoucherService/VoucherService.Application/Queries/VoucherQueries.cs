using MediatR;
using VoucherService.Domain.Entities;

namespace VoucherService.Application.Queries;

// Get Voucher by ID
public record GetVoucherByIdQuery(string VoucherId) : IRequest<VoucherDto?>;

// Get Active Vouchers (with pagination)
public record GetActiveVouchersQuery(
    int Page = 1,
    int PageSize = 20,
    Platform? Platform = null
) : IRequest<VouchersResult>;

public record VouchersResult(
    IReadOnlyList<VoucherDto> Vouchers,
    int TotalCount,
    int Page,
    int PageSize
);

// Get Expiring Soon Vouchers
public record GetExpiringSoonQuery(int HoursAhead = 24) : IRequest<IReadOnlyList<VoucherDto>>;

// Get Vouchers by Post
public record GetVouchersByPostQuery(string PostId) : IRequest<IReadOnlyList<VoucherDto>>;

// Voucher DTO
public record VoucherDto(
    string Id,
    string Code,
    string? Description,
    string Platform,
    string Type,
    decimal? DiscountValue,
    decimal? DiscountPercent,
    decimal? MinSpend,
    decimal? MaxDiscount,
    DateTime StartAt,
    DateTime ExpiresAt,
    int RemainingPercent,
    string Urgency,
    string Status,
    int UsageCount,
    int ViewCount,
    string? PostId
);
