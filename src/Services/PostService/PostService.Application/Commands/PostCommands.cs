using MediatR;
using PostService.Domain.Entities;

namespace PostService.Application.Commands;

// Create Post Command
public record CreatePostCommand(
    string Title,
    string AuthorId,
    Platform Platform,
    string OriginalUrl,
    string? Description,
    decimal? OriginalPrice,
    decimal? SalePrice,
    DateTime? VoucherExpiresAt,
    List<string>? MediaObjectKeys
) : IRequest<CreatePostResult>;

public record CreatePostResult(string PostId, bool Success, string? Error = null);

// Update Post Command
public record UpdatePostCommand(
    string PostId,
    string Title,
    string? Description,
    decimal? OriginalPrice,
    decimal? SalePrice,
    DateTime? VoucherExpiresAt,
    List<string>? MediaObjectKeys
) : IRequest<bool>;

// Publish Post Command
public record PublishPostCommand(string PostId) : IRequest<bool>;
// Delete Post Command
public record DeletePostCommand(string PostId) : IRequest<bool>;
// Set Affiliate URL Command
public record SetAffiliateUrlCommand(string PostId, string AffiliateUrl) : IRequest<bool>;
