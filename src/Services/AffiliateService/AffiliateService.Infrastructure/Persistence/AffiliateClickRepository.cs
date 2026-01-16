using AffiliateService.Application.Interfaces;
using AffiliateService.Domain.Entities;
using MongoDB.Driver;

namespace AffiliateService.Infrastructure.Persistence;

public class AffiliateClickRepository : IAffiliateClickRepository
{
    private readonly IMongoCollection<AffiliateClick> _collection;

    public AffiliateClickRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<AffiliateClick>("affiliate_clicks");

        // Create indexes
        var postIdIndex = new CreateIndexModel<AffiliateClick>(
            Builders<AffiliateClick>.IndexKeys.Ascending(x => x.PostId).Descending(x => x.ClickedAt));
        _collection.Indexes.CreateOne(postIdIndex);

        var platformIndex = new CreateIndexModel<AffiliateClick>(
            Builders<AffiliateClick>.IndexKeys.Ascending(x => x.Platform).Descending(x => x.ClickedAt));
        _collection.Indexes.CreateOne(platformIndex);

        var timeIndex = new CreateIndexModel<AffiliateClick>(
            Builders<AffiliateClick>.IndexKeys.Descending(x => x.ClickedAt));
        _collection.Indexes.CreateOne(timeIndex);
    }

    public async Task<AffiliateClick?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AffiliateClick>> GetByPostIdAsync(string postId, int take = 100, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(x => x.PostId == postId)
            .SortByDescending(x => x.ClickedAt)
            .Limit(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetClickCountByPostAsync(string postId, CancellationToken cancellationToken = default)
    {
        return (int)await _collection.CountDocumentsAsync(x => x.PostId == postId, cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<AffiliateClick>> GetByPlatformAsync(Platform platform, DateTime from, DateTime to, int take = 1000, CancellationToken cancellationToken = default)
    {
        var filter = Builders<AffiliateClick>.Filter.Eq(x => x.Platform, platform)
            & Builders<AffiliateClick>.Filter.Gte(x => x.ClickedAt, from)
            & Builders<AffiliateClick>.Filter.Lt(x => x.ClickedAt, to);

        return await _collection
            .Find(filter)
            .SortByDescending(x => x.ClickedAt)
            .Limit(take)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(AffiliateClick click, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(click, cancellationToken: cancellationToken);
    }
}
