using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Infrastructure.Redis;

/// <summary>
/// Redis cache service for caching and rate limiting.
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    Task<long> IncrementAsync(string key, long value = 1, CancellationToken cancellationToken = default);
    Task<bool> SetNxAsync(string key, string value, TimeSpan expiry, CancellationToken cancellationToken = default);
}

/// <summary>
/// Redis implementation of cache service.
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(
        IConnectionMultiplexer redis,
        ILogger<RedisCacheService> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await _database.StringGetAsync(key);
            if (value.IsNullOrEmpty)
                return default;

            return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache key: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(value, _jsonOptions);
            await _database.StringSetAsync(
                key: key,
                value: json,
                expiry: expiry,
                when: When.Always,
                flags: CommandFlags.None
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache key: {Key}", key);
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _database.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking cache key: {Key}", key);
            return false;
        }
    }

    public async Task<long> IncrementAsync(string key, long value = 1, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _database.StringIncrementAsync(key, value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing cache key: {Key}", key);
            return 0;
        }
    }

    public async Task<bool> SetNxAsync(string key, string value, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _database.StringSetAsync(key, value, expiry, When.NotExists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting NX cache key: {Key}", key);
            return false;
        }
    }
}
