using MediatR;
using VoucherService.Domain.Entities;

namespace VoucherService.Application.Commands;

// Create Voucher Command
public record CreateVoucherCommand(
    string Code,
    Platform Platform,
    VoucherType Type,
    DateTime StartAt,
    DateTime ExpiresAt,
    string? Description,
    decimal? DiscountValue,
    decimal? DiscountPercent,
    decimal? MinSpend,
    decimal? MaxDiscount,
    string? PostId
) : IRequest<CreateVoucherResult>;

public record CreateVoucherResult(string VoucherId, bool Success, string? Error = null);

// Update Voucher Command
public record UpdateVoucherCommand(
    string VoucherId,
    string? Description,
    decimal? DiscountValue,
    decimal? DiscountPercent,
    decimal? MinSpend,
    decimal? MaxDiscount,
    DateTime? ExpiresAt
) : IRequest<bool>;

// Deactivate Voucher Command
public record DeactivateVoucherCommand(string VoucherId) : IRequest<bool>;

// Link Voucher to Post
public record LinkVoucherToPostCommand(string VoucherId, string PostId) : IRequest<bool>;

// Track Voucher Usage
public record TrackVoucherUsageCommand(string VoucherId) : IRequest<bool>;

// Expire Vouchers (Background Job)
public record ExpireVouchersCommand : IRequest<int>;
