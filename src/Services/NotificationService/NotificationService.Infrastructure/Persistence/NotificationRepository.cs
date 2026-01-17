using MongoDB.Driver;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Persistence;

public class NotificationRepository : INotificationRepository
{
    private readonly IMongoCollection<Notification> _collection;

    public NotificationRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Notification>("notifications");

        // Create indexes
        var userIdIndex = new CreateIndexModel<Notification>(
            Builders<Notification>.IndexKeys.Ascending(x => x.UserId).Descending(x => x.CreatedAt));
        _collection.Indexes.CreateOne(userIdIndex);

        var statusIndex = new CreateIndexModel<Notification>(
            Builders<Notification>.IndexKeys.Ascending(x => x.UserId).Ascending(x => x.Status));
        _collection.Indexes.CreateOne(statusIndex);
    }

    public async Task<Notification?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> GetByUserIdAsync(string userId, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(x => x.UserId == userId)
            .SortByDescending(x => x.CreatedAt)
            .Skip(skip)
            .Limit(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(string userId, CancellationToken cancellationToken = default)
    {
        return (int)await _collection.CountDocumentsAsync(
            x => x.UserId == userId && x.ReadAt == null,
            cancellationToken: cancellationToken);
    }

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(notification, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await _collection.ReplaceOneAsync(x => x.Id == notification.Id, notification, cancellationToken: cancellationToken);
    }

    public async Task MarkAllReadAsync(string userId, CancellationToken cancellationToken = default)
    {
        var update = Builders<Notification>.Update.Set(x => x.ReadAt, DateTime.UtcNow);
        await _collection.UpdateManyAsync(
            x => x.UserId == userId && x.ReadAt == null,
            update,
            cancellationToken: cancellationToken);
    }
}
