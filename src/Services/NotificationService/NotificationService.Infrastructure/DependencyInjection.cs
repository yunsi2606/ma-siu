using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NotificationService.Application.Interfaces;
using NotificationService.Application.Services;
using NotificationService.Infrastructure.Firebase;
using NotificationService.Infrastructure.Persistence;
using NotificationService.Infrastructure.Redis;
using StackExchange.Redis;

namespace NotificationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string mongoConnectionString,
        string mongoDatabaseName,
        string redisConnectionString)
    {
        // MongoDB
        services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnectionString));
        services.AddScoped<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoDatabaseName);
        });

        // Redis
        services.AddSingleton<IConnectionMultiplexer>(_ => 
            ConnectionMultiplexer.Connect(redisConnectionString));

        // Repositories
        services.AddScoped<INotificationRepository, NotificationRepository>();
        
        // Services
        services.AddScoped<IFcmService, FcmService>();
        services.AddScoped<IDeduplicationService, RedisDeduplicationService>();
        services.AddScoped<NotificationSenderService>();

        return services;
    }
}
