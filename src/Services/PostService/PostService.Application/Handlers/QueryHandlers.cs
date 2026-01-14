using MediatR;
using PostService.Application.Interfaces;
using PostService.Application.Queries;
using PostService.Application.Services;
using PostService.Domain.Entities;

namespace PostService.Application.Handlers;

public class GetPostByIdHandler : IRequestHandler<GetPostByIdQuery, PostDto?>
{
    private readonly IPostRepository _postRepository;
    private readonly IStorageUrlProvider _storageUrlProvider;

    public GetPostByIdHandler(IPostRepository postRepository, IStorageUrlProvider storageUrlProvider)
    {
        _postRepository = postRepository;
        _storageUrlProvider = storageUrlProvider;
    }

    public async Task<PostDto?> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null) return null;

        post.IncrementViewCount();
        await _postRepository.UpdateAsync(post, cancellationToken);

        return MapToDto(post);
    }

    private PostDto MapToDto(Post post)
    {
        int? voucherRemainingPercent = null;
        if (post.VoucherExpiresAt.HasValue)
        {
            voucherRemainingPercent = VoucherRemainingCalculator.CalculateRemainingPercent(
                post.CreatedAt, post.VoucherExpiresAt.Value);
        }

        return new PostDto(
            post.Id,
            post.Title,
            post.Description,
            post.AuthorId,
            post.Platform.ToString(),
            post.OriginalUrl,
            post.AffiliateUrl,
            post.OriginalPrice,
            post.SalePrice,
            post.DiscountPercent,
            post.VoucherExpiresAt,
            voucherRemainingPercent,
            post.Status.ToString(),
            post.CreatedAt,
            post.PublishedAt,
            post.Media.Select(m => new MediaDto(
                _storageUrlProvider.GetPublicUrl(m.ObjectKey),
                m.Type.ToString()
            )).ToList(),
            post.ViewCount,
            post.ClickCount
        );
    }
}

public class GetFeedHandler : IRequestHandler<GetFeedQuery, FeedResult>
{
    private readonly IPostRepository _postRepository;
    private readonly IStorageUrlProvider _storageUrlProvider;

    public GetFeedHandler(IPostRepository postRepository, IStorageUrlProvider storageUrlProvider)
    {
        _postRepository = postRepository;
        _storageUrlProvider = storageUrlProvider;
    }

    public async Task<FeedResult> Handle(GetFeedQuery request, CancellationToken cancellationToken)
    {
        var skip = (request.Page - 1) * request.PageSize;
        var posts = await _postRepository.GetPublishedAsync(skip, request.PageSize, request.Platform, cancellationToken);
        var totalCount = await _postRepository.GetPublishedCountAsync(request.Platform, cancellationToken);

        var postDtos = posts.Select(MapToDto).ToList();

        return new FeedResult(postDtos, totalCount, request.Page, request.PageSize);
    }

    private PostDto MapToDto(Post post)
    {
        int? voucherRemainingPercent = null;
        if (post.VoucherExpiresAt.HasValue)
        {
            voucherRemainingPercent = VoucherRemainingCalculator.CalculateRemainingPercent(
                post.CreatedAt, post.VoucherExpiresAt.Value);
        }

        return new PostDto(
            post.Id,
            post.Title,
            post.Description,
            post.AuthorId,
            post.Platform.ToString(),
            post.OriginalUrl,
            post.AffiliateUrl,
            post.OriginalPrice,
            post.SalePrice,
            post.DiscountPercent,
            post.VoucherExpiresAt,
            voucherRemainingPercent,
            post.Status.ToString(),
            post.CreatedAt,
            post.PublishedAt,
            post.Media.Select(m => new MediaDto(
                _storageUrlProvider.GetPublicUrl(m.ObjectKey),
                m.Type.ToString()
            )).ToList(),
            post.ViewCount,
            post.ClickCount
        );
    }
}

public class GetPostsByAuthorHandler : IRequestHandler<GetPostsByAuthorQuery, IReadOnlyList<PostDto>>
{
    private readonly IPostRepository _postRepository;
    private readonly IStorageUrlProvider _storageUrlProvider;

    public GetPostsByAuthorHandler(IPostRepository postRepository, IStorageUrlProvider storageUrlProvider)
    {
        _postRepository = postRepository;
        _storageUrlProvider = storageUrlProvider;
    }

    public async Task<IReadOnlyList<PostDto>> Handle(GetPostsByAuthorQuery request, CancellationToken cancellationToken)
    {
        var posts = await _postRepository.GetByAuthorAsync(request.AuthorId, cancellationToken);
        return posts.Select(MapToDto).ToList();
    }

    private PostDto MapToDto(Post post)
    {
        int? voucherRemainingPercent = null;
        if (post.VoucherExpiresAt.HasValue)
        {
            voucherRemainingPercent = VoucherRemainingCalculator.CalculateRemainingPercent(
                post.CreatedAt, post.VoucherExpiresAt.Value);
        }

        return new PostDto(
            post.Id,
            post.Title,
            post.Description,
            post.AuthorId,
            post.Platform.ToString(),
            post.OriginalUrl,
            post.AffiliateUrl,
            post.OriginalPrice,
            post.SalePrice,
            post.DiscountPercent,
            post.VoucherExpiresAt,
            voucherRemainingPercent,
            post.Status.ToString(),
            post.CreatedAt,
            post.PublishedAt,
            post.Media.Select(m => new MediaDto(
                _storageUrlProvider.GetPublicUrl(m.ObjectKey),
                m.Type.ToString()
            )).ToList(),
            post.ViewCount,
            post.ClickCount
        );
    }
}
