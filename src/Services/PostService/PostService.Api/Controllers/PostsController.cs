using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostService.Application.Commands;
using PostService.Application.Interfaces;
using PostService.Application.Queries;
using PostService.Domain.Entities;

namespace PostService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IStorageUrlProvider _storageUrlProvider;
    private readonly ILogger<PostsController> _logger;

    public PostsController(
        IMediator mediator,
        IStorageUrlProvider storageUrlProvider,
        ILogger<PostsController> logger)
    {
        _mediator = mediator;
        _storageUrlProvider = storageUrlProvider;
        _logger = logger;
    }

    /// <summary>
    /// Get feed of published posts with pagination.
    /// </summary>
    [HttpGet("feed")]
    public async Task<ActionResult<FeedResult>> GetFeed(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? platform = null,
        CancellationToken cancellationToken = default)
    {
        Platform? platformFilter = null;
        if (!string.IsNullOrEmpty(platform) && Enum.TryParse<Platform>(platform, true, out var p))
        {
            platformFilter = p;
        }

        var query = new GetFeedQuery(page, pageSize, platformFilter);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get post by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PostDto>> GetById(string id, CancellationToken cancellationToken)
    {
        var query = new GetPostByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Create new post (Admin only).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CreatePostResult>> Create(
        [FromBody] CreatePostRequest request,
        CancellationToken cancellationToken)
    {
        var authorId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(authorId))
            return Unauthorized();

        if (!Enum.TryParse<Platform>(request.Platform, true, out var platform))
            return BadRequest(new { message = "Invalid platform" });

        var command = new CreatePostCommand(
            request.Title,
            authorId,
            platform,
            request.OriginalUrl,
            request.Description,
            request.OriginalPrice,
            request.SalePrice,
            request.VoucherExpiresAt,
            request.MediaObjectKeys
        );

        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result.Success)
            return BadRequest(new { message = result.Error });

        _logger.LogInformation("Post {PostId} created by {AuthorId}", result.PostId, authorId);
        return CreatedAtAction(nameof(GetById), new { id = result.PostId }, result);
    }

    /// <summary>
    /// Publish post (Admin only).
    /// </summary>
    [HttpPost("{id}/publish")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Publish(string id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new PublishPostCommand(id), cancellationToken);
        
        if (!result)
            return NotFound();

        _logger.LogInformation("Post {PostId} published", id);
        return NoContent();
    }

    /// <summary>
    /// Delete post (Admin only).
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeletePostCommand(id), cancellationToken);
        
        if (!result)
            return NotFound();

        _logger.LogInformation("Post {PostId} deleted", id);
        return NoContent();
    }

    /// <summary>
    /// Get presigned URL for uploading image to MinIO.
    /// </summary>
    [HttpGet("upload-url")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UploadUrlResponse>> GetUploadUrl(
        [FromQuery] string filename,
        CancellationToken cancellationToken)
    {
        var objectKey = $"{Guid.NewGuid()}/{filename}";
        var url = await _storageUrlProvider.GetPresignedUploadUrlAsync(objectKey);

        return Ok(new UploadUrlResponse(url, objectKey));
    }

    /// <summary>
    /// Health check.
    /// </summary>
    [HttpGet("/health")]
    public IActionResult Health() => Ok(new { status = "healthy", service = "post-service" });
}

// Request DTOs
public record CreatePostRequest(
    string Title,
    string Platform,
    string OriginalUrl,
    string? Description,
    decimal? OriginalPrice,
    decimal? SalePrice,
    DateTime? VoucherExpiresAt,
    List<string>? MediaObjectKeys
);

public record UploadUrlResponse(string UploadUrl, string ObjectKey);
