namespace PostService.Application.Interfaces;

/// <summary>
/// Interface for generating public URLs for stored objects.
/// Extends shared IStorageService for bucket-specific operations.
/// </summary>
public interface IStorageUrlProvider
{
    string GetPublicUrl(string objectKey);
    Task<string> GetPresignedUploadUrlAsync(string objectKey, int expirySeconds = 3600);
    string BucketName { get; }
}
