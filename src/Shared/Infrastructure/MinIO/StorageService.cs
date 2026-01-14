using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Infrastructure.MinIO;

/// <summary>
/// MinIO configuration options.
/// </summary>
public class MinIOOptions
{
    public const string SectionName = "MinIO";
    
    public string Endpoint { get; set; } = "localhost:9000";
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public bool UseSSL { get; set; }
    public string PublicEndpoint { get; set; } = string.Empty;
}

/// <summary>
/// MinIO storage service for file operations with pre-signed URLs.
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Generates a pre-signed URL for uploading a file.
    /// Client uploads directly to MinIO using this URL.
    /// </summary>
    Task<string> GetPresignedUploadUrlAsync(
        string bucketName,
        string objectName,
        int expirySeconds = 3600,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a pre-signed URL for downloading/viewing a file.
    /// </summary>
    Task<string> GetPresignedDownloadUrlAsync(
        string bucketName,
        string objectName,
        int expirySeconds = 3600,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the public URL for an object (if bucket is public).
    /// </summary>
    string GetPublicUrl(string bucketName, string objectName);

    /// <summary>
    /// Deletes an object from storage.
    /// </summary>
    Task DeleteAsync(
        string bucketName,
        string objectName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an object exists.
    /// </summary>
    Task<bool> ExistsAsync(
        string bucketName,
        string objectName,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// MinIO implementation of storage service.
/// </summary>
public class MinIOStorageService : IStorageService
{
    private readonly IMinioClient _client;
    private readonly MinIOOptions _options;
    private readonly ILogger<MinIOStorageService> _logger;

    public MinIOStorageService(
        IMinioClient client,
        IOptions<MinIOOptions> options,
        ILogger<MinIOStorageService> logger)
    {
        _client = client;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> GetPresignedUploadUrlAsync(
        string bucketName,
        string objectName,
        int expirySeconds = 3600,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var args = new PresignedPutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithExpiry(expirySeconds);

            return await _client.PresignedPutObjectAsync(args);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate presigned upload URL for {Bucket}/{Object}", 
                bucketName, objectName);
            throw;
        }
    }

    public async Task<string> GetPresignedDownloadUrlAsync(
        string bucketName,
        string objectName,
        int expirySeconds = 3600,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var args = new PresignedGetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithExpiry(expirySeconds);

            return await _client.PresignedGetObjectAsync(args);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate presigned download URL for {Bucket}/{Object}", 
                bucketName, objectName);
            throw;
        }
    }

    public string GetPublicUrl(string bucketName, string objectName)
    {
        var baseUrl = _options.PublicEndpoint.TrimEnd('/');
        return $"{baseUrl}/{bucketName}/{objectName}";
    }

    public async Task DeleteAsync(
        string bucketName,
        string objectName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var args = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);

            await _client.RemoveObjectAsync(args, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete {Bucket}/{Object}", bucketName, objectName);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(
        string bucketName,
        string objectName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var args = new StatObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);

            await _client.StatObjectAsync(args, cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
