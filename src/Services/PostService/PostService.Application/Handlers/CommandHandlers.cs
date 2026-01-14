using MediatR;
using PostService.Application.Commands;
using PostService.Application.Interfaces;
using PostService.Domain.Entities;

namespace PostService.Application.Handlers;

public class CreatePostHandler : IRequestHandler<CreatePostCommand, CreatePostResult>
{
    private readonly IPostRepository _postRepository;

    public CreatePostHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<CreatePostResult> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var post = Post.Create(
                request.Title,
                request.AuthorId,
                request.Platform,
                request.OriginalUrl,
                request.Description
            );

            post.Update(
                request.Title,
                request.Description,
                request.OriginalPrice,
                request.SalePrice,
                request.VoucherExpiresAt
            );

            if (request.MediaObjectKeys != null)
            {
                foreach (var (key, index) in request.MediaObjectKeys.Select((k, i) => (k, i)))
                {
                    post.AddMedia(key, MediaType.Image, index);
                }
            }

            await _postRepository.AddAsync(post, cancellationToken);

            return new CreatePostResult(post.Id, true);
        }
        catch (Exception ex)
        {
            return new CreatePostResult(string.Empty, false, ex.Message);
        }
    }
}

public class UpdatePostHandler : IRequestHandler<UpdatePostCommand, bool>
{
    private readonly IPostRepository _postRepository;

    public UpdatePostHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<bool> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null) return false;

        post.Update(
            request.Title,
            request.Description,
            request.OriginalPrice,
            request.SalePrice,
            request.VoucherExpiresAt
        );

        if (request.MediaObjectKeys != null)
        {
            post.ClearMedia();
            foreach (var (key, index) in request.MediaObjectKeys.Select((k, i) => (k, i)))
            {
                post.AddMedia(key, MediaType.Image, index);
            }
        }

        await _postRepository.UpdateAsync(post, cancellationToken);
        return true;
    }
}

public class PublishPostHandler : IRequestHandler<PublishPostCommand, bool>
{
    private readonly IPostRepository _postRepository;

    public PublishPostHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<bool> Handle(PublishPostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null) return false;

        post.Publish();
        await _postRepository.UpdateAsync(post, cancellationToken);

        // TODO: Publish PostCreatedEvent to RabbitMQ for notifications
        
        return true;
    }
}

public class DeletePostHandler : IRequestHandler<DeletePostCommand, bool>
{
    private readonly IPostRepository _postRepository;

    public DeletePostHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<bool> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null) return false;

        await _postRepository.DeleteAsync(request.PostId, cancellationToken);
        return true;
    }
}

public class SetAffiliateUrlHandler : IRequestHandler<SetAffiliateUrlCommand, bool>
{
    private readonly IPostRepository _postRepository;

    public SetAffiliateUrlHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<bool> Handle(SetAffiliateUrlCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null) return false;

        post.SetAffiliateUrl(request.AffiliateUrl);
        await _postRepository.UpdateAsync(post, cancellationToken);
        return true;
    }
}
