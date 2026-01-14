using Infrastructure.MinIO;
using Microsoft.Extensions.DependencyInjection;
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
        Action<MinIOOptions> configureMinIO,
        Action<PostStorageOptions>? configurePostStorage = null)
    {
        // MongoDB
        services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnectionString));
        services.AddScoped<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoDatabaseName);
        });

        // Shared MinIO from Infrastructure
        services.AddMinIOStorage(configureMinIO);

        // Post-specific storage config
        if (configurePostStorage != null)
            services.Configure(configurePostStorage);
        else
            services.Configure<PostStorageOptions>(_ => { });

        // Repositories
        services.AddScoped<IPostRepository, PostRepository>();
        
        // Storage adapter using shared IStorageService
        services.AddScoped<IStorageUrlProvider, PostStorageUrlProvider>();

        return services;
    }
}
