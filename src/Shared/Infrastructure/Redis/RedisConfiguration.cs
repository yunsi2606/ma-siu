using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure.Redis;

/// <summary>
/// Redis configuration and service registration.
/// </summary>
public static class RedisConfiguration
{
    /// <summary>
    /// Adds Redis services to the DI container.
    /// </summary>
    public static IServiceCollection AddRedisCache(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(connectionString));

        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }
}
