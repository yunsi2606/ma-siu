using System.Linq.Expressions;
using BuildingBlocks.Abstractions;
using MongoDB.Driver;

namespace Infrastructure.MongoDB;

/// <summary>
/// Generic MongoDB repository implementation.
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class MongoRepository<T> : IRepository<T> where T : Entity
{
    protected readonly IMongoCollection<T> Collection;

    public MongoRepository(IMongoDatabase database, string collectionName)
    {
        Collection = database.GetCollection<T>(collectionName);
    }

    public async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await Collection.Find(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Collection.Find(_ => true)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await Collection.Find(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task<T?> FindOneAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await Collection.Find(predicate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await Collection.Find(predicate)
            .AnyAsync(cancellationToken);
    }

    public async Task<long> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        return predicate == null
            ? await Collection.CountDocumentsAsync(_ => true, cancellationToken: cancellationToken)
            : await Collection.CountDocumentsAsync(predicate, cancellationToken: cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await Collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await Collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await Collection.ReplaceOneAsync(
            x => x.Id == entity.Id,
            entity,
            cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        await Collection.DeleteOneAsync(x => x.Id == entity.Id, cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await Collection.DeleteOneAsync(x => x.Id == id, cancellationToken);
    }
}
