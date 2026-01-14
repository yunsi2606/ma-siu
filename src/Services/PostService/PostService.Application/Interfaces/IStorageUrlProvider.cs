namespace PostService.Application.Interfaces;

/// <summary>
/// Interface for generating public URLs for stored objects.
/// </summary>
public interface IStorageUrlProvider
{
    string GetPublicUrl(string objectKey);
    Task<string> GetPresignedUploadUrlAsync(string objectKey, int expirySeconds = 3600);
}
