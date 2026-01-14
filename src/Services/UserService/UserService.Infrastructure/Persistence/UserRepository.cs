using MongoDB.Driver;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _collection;

    public UserRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<User>("users");
        
        // Create indexes
        var googleIdIndex = new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(x => x.GoogleId),
            new CreateIndexOptions { Unique = true });
        _collection.Indexes.CreateOne(googleIdIndex);
        
        var emailIndex = new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(x => x.Email));
        _collection.Indexes.CreateOne(emailIndex);
    }

    public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.GoogleId == googleId).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Email == email).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.IsActive).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(user, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        await _collection.ReplaceOneAsync(x => x.Id == user.Id, user, cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsAsync(string googleId, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.GoogleId == googleId).AnyAsync(cancellationToken);
    }
}
