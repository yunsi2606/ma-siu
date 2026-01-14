using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using PostService.Application.Interfaces;

namespace PostService.Infrastructure.Storage;

public class MinIOOptions
{
    public const string SectionName = "MinIO";
    
    public string Endpoint { get; set; } = "localhost:9000";
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public bool UseSSL { get; set; }
    public string PublicEndpoint { get; set; } = string.Empty;
    public string PostsBucket { get; set; } = "posts";
}

public class MinIOStorageUrlProvider : IStorageUrlProvider
{
    private readonly IMinioClient _client;
    private readonly MinIOOptions _options;

    public MinIOStorageUrlProvider(IMinioClient client, IOptions<MinIOOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public string GetPublicUrl(string objectKey)
    {
        var baseUrl = _options.PublicEndpoint.TrimEnd('/');
        return $"{baseUrl}/{_options.PostsBucket}/{objectKey}";
    }

    public async Task<string> GetPresignedUploadUrlAsync(string objectKey, int expirySeconds = 3600)
    {
        var args = new PresignedPutObjectArgs()
            .WithBucket(_options.PostsBucket)
            .WithObject(objectKey)
            .WithExpiry(expirySeconds);

        return await _client.PresignedPutObjectAsync(args);
    }
}
