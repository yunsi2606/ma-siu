using MongoDB.Driver;
using PostService.Application.Interfaces;
using PostService.Domain.Entities;

namespace PostService.Infrastructure.Persistence;

public class PostRepository : IPostRepository
{
    private readonly IMongoCollection<Post> _collection;

    public PostRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Post>("posts");
        
        // Create indexes
        var authorIndex = new CreateIndexModel<Post>(
            Builders<Post>.IndexKeys.Ascending(x => x.AuthorId));
        _collection.Indexes.CreateOne(authorIndex);
        
        var statusIndex = new CreateIndexModel<Post>(
            Builders<Post>.IndexKeys.Ascending(x => x.Status).Descending(x => x.PublishedAt));
        _collection.Indexes.CreateOne(statusIndex);
        
        var platformIndex = new CreateIndexModel<Post>(
            Builders<Post>.IndexKeys.Ascending(x => x.Platform));
        _collection.Indexes.CreateOne(platformIndex);
    }

    public async Task<Post?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Post>> GetPublishedAsync(int skip, int take, Platform? platform = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Post>.Filter.Eq(x => x.Status, PostStatus.Published);
        if (platform.HasValue)
        {
            filter = filter & Builders<Post>.Filter.Eq(x => x.Platform, platform.Value);
        }

        return await _collection
            .Find(filter)
            .SortByDescending(x => x.PublishedAt)
            .Skip(skip)
            .Limit(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetPublishedCountAsync(Platform? platform = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Post>.Filter.Eq(x => x.Status, PostStatus.Published);
        if (platform.HasValue)
        {
            filter = filter & Builders<Post>.Filter.Eq(x => x.Platform, platform.Value);
        }

        return (int)await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<Post>> GetByAuthorAsync(string authorId, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(x => x.AuthorId == authorId)
            .SortByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Post post, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(post, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(Post post, CancellationToken cancellationToken = default)
    {
        await _collection.ReplaceOneAsync(x => x.Id == post.Id, post, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(x => x.Id == id, cancellationToken);
    }
}
