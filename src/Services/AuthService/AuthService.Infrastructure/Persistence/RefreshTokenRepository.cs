using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using MongoDB.Driver;

namespace AuthService.Infrastructure.Persistence;

/// <summary>
/// MongoDB repository for refresh tokens.
/// </summary>
public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IMongoCollection<RefreshToken> _collection;

    public RefreshTokenRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<RefreshToken>("refresh_tokens");
        
        // Create index for token lookup
        var tokenIndex = new CreateIndexModel<RefreshToken>(
            Builders<RefreshToken>.IndexKeys.Ascending(x => x.Token));
        _collection.Indexes.CreateOne(tokenIndex);
        
        // Create index for user lookup
        var userIndex = new CreateIndexModel<RefreshToken>(
            Builders<RefreshToken>.IndexKeys.Ascending(x => x.UserId));
        _collection.Indexes.CreateOne(userIndex);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(x => x.Token == token)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RefreshToken>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(refreshToken, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        await _collection.ReplaceOneAsync(
            x => x.Id == refreshToken.Id,
            refreshToken,
            cancellationToken: cancellationToken);
    }

    public async Task RevokeAllForUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var update = Builders<RefreshToken>.Update
            .Set(x => x.IsRevoked, true)
            .Set(x => x.RevokedAt, DateTime.UtcNow);

        await _collection.UpdateManyAsync(
            x => x.UserId == userId && !x.IsRevoked,
            update,
            cancellationToken: cancellationToken);
    }
}
