using NotificationService.Application.Interfaces;
using StackExchange.Redis;

namespace NotificationService.Infrastructure.Redis;

/// <summary>
/// Redis-based notification deduplication service.
/// Prevents sending duplicate notifications.
/// </summary>
public class RedisDeduplicationService : IDeduplicationService
{
    private readonly IDatabase _redis;
    private const string KeyPrefix = "notif:dedup:";

    public RedisDeduplicationService(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
    }

    public async Task<bool> IsDuplicateAsync(string userId, string key, CancellationToken cancellationToken = default)
    {
        var redisKey = $"{KeyPrefix}{userId}:{key}";
        return await _redis.KeyExistsAsync(redisKey);
    }

    public async Task MarkSentAsync(string userId, string key, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        var redisKey = $"{KeyPrefix}{userId}:{key}";
        await _redis.StringSetAsync(redisKey, "1", ttl);
    }
}
