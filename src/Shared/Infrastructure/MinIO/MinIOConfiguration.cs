using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace Infrastructure.MinIO;

/// <summary>
/// MinIO configuration and service registration.
/// </summary>
public static class MinIOConfiguration
{
    /// <summary>
    /// Adds MinIO services to the DI container.
    /// </summary>
    public static IServiceCollection AddMinIOStorage(
        this IServiceCollection services,
        Action<MinIOOptions> configureOptions)
    {
        services.Configure(configureOptions);

        services.AddSingleton<IMinioClient>(sp =>
        {
            var options = new MinIOOptions();
            configureOptions(options);

            var builder = new MinioClient()
                .WithEndpoint(options.Endpoint)
                .WithCredentials(options.AccessKey, options.SecretKey);

            if (options.UseSSL)
                builder = builder.WithSSL();

            return builder.Build();
        });

        services.AddScoped<IStorageService, MinIOStorageService>();

        return services;
    }
}
