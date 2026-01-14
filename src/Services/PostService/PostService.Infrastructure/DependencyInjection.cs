using Microsoft.Extensions.DependencyInjection;
using Minio;
using MongoDB.Driver;
using PostService.Application.Interfaces;
using PostService.Infrastructure.Persistence;
using PostService.Infrastructure.Storage;

namespace PostService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string mongoConnectionString,
        string mongoDatabaseName,
        Action<MinIOOptions> configureMinIO)
    {
        // MongoDB
        services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnectionString));
        services.AddScoped<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoDatabaseName);
        });

        // MinIO
        services.Configure(configureMinIO);
        services.AddSingleton<IMinioClient>(sp =>
        {
            var options = new MinIOOptions();
            configureMinIO(options);

            var builder = new MinioClient()
                .WithEndpoint(options.Endpoint)
                .WithCredentials(options.AccessKey, options.SecretKey);

            if (options.UseSSL)
                builder = builder.WithSSL();

            return builder.Build();
        });

        // Repositories
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IStorageUrlProvider, MinIOStorageUrlProvider>();

        return services;
    }
}
