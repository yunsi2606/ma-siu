using Infrastructure.MinIO;
using Microsoft.Extensions.Options;
using PostService.Application.Interfaces;

namespace PostService.Infrastructure.Storage;

/// <summary>
/// Adapts shared IStorageService for PostService bucket.
/// Uses Shared/Infrastructure MinIO implementation.
/// </summary>
public class PostStorageUrlProvider : IStorageUrlProvider
{
    private readonly IStorageService _storageService;
    private readonly string _bucketName;

    public PostStorageUrlProvider(
        IStorageService storageService,
        IOptions<PostStorageOptions> options)
    {
        _storageService = storageService;
        _bucketName = options.Value.BucketName;
    }

    public string BucketName => _bucketName;

    public string GetPublicUrl(string objectKey)
    {
        return _storageService.GetPublicUrl(_bucketName, objectKey);
    }

    public async Task<string> GetPresignedUploadUrlAsync(string objectKey, int expirySeconds = 3600)
    {
        return await _storageService.GetPresignedUploadUrlAsync(_bucketName, objectKey, expirySeconds);
    }
}

public class PostStorageOptions
{
    public const string SectionName = "PostStorage";
    public string BucketName { get; set; } = "posts";
}
