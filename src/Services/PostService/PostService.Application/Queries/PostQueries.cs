using MediatR;
using PostService.Domain.Entities;

namespace PostService.Application.Queries;
// Get Post by ID
public record GetPostByIdQuery(string PostId) : IRequest<PostDto?>;

// Get Feed (Published Posts)
public record GetFeedQuery(
    int Page = 1,
    int PageSize = 20,
    Platform? Platform = null
) : IRequest<FeedResult>;

public record FeedResult(
    IReadOnlyList<PostDto> Posts,
    int TotalCount,
    int Page,
    int PageSize
);

// Get Posts by Author
public record GetPostsByAuthorQuery(string AuthorId) : IRequest<IReadOnlyList<PostDto>>;
// Post DTO
public record PostDto(
    string Id,
    string Title,
    string? Description,
    string AuthorId,
    string Platform,
    string OriginalUrl,
    string? AffiliateUrl,
    decimal? OriginalPrice,
    decimal? SalePrice,
    int? DiscountPercent,
    DateTime? VoucherExpiresAt,
    int? VoucherRemainingPercent,  // Time-based decay
    string Status,
    DateTime CreatedAt,
    DateTime? PublishedAt,
    IReadOnlyList<MediaDto> Media,
    int ViewCount,
    int ClickCount
);

public record MediaDto(string Url, string Type);
