using MongoDB.Driver;
using VoucherService.Application.Interfaces;
using VoucherService.Domain.Entities;

namespace VoucherService.Infrastructure.Persistence;

public class VoucherRepository : IVoucherRepository
{
    private readonly IMongoCollection<Voucher> _collection;

    public VoucherRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Voucher>("vouchers");
        
        // Create indexes
        var codeIndex = new CreateIndexModel<Voucher>(
            Builders<Voucher>.IndexKeys.Ascending(x => x.Code).Ascending(x => x.Platform),
            new CreateIndexOptions { Unique = true });
        _collection.Indexes.CreateOne(codeIndex);
        
        var expiryIndex = new CreateIndexModel<Voucher>(
            Builders<Voucher>.IndexKeys.Ascending(x => x.ExpiresAt));
        _collection.Indexes.CreateOne(expiryIndex);
        
        var statusIndex = new CreateIndexModel<Voucher>(
            Builders<Voucher>.IndexKeys.Ascending(x => x.Status).Descending(x => x.CreatedAt));
        _collection.Indexes.CreateOne(statusIndex);
        
        var postIndex = new CreateIndexModel<Voucher>(
            Builders<Voucher>.IndexKeys.Ascending(x => x.PostId));
        _collection.Indexes.CreateOne(postIndex);
    }

    public async Task<Voucher?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Voucher?> GetByCodeAsync(string code, Platform platform, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Code == code && x.Platform == platform).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Voucher>> GetActiveAsync(int skip, int take, Platform? platform = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Voucher>.Filter.Eq(x => x.Status, VoucherStatus.Active)
            & Builders<Voucher>.Filter.Gt(x => x.ExpiresAt, DateTime.UtcNow);

        if (platform.HasValue)
            filter &= Builders<Voucher>.Filter.Eq(x => x.Platform, platform.Value);

        return await _collection
            .Find(filter)
            .SortByDescending(x => x.CreatedAt)
            .Skip(skip)
            .Limit(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetActiveCountAsync(Platform? platform = null, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Voucher>.Filter.Eq(x => x.Status, VoucherStatus.Active)
            & Builders<Voucher>.Filter.Gt(x => x.ExpiresAt, DateTime.UtcNow);

        if (platform.HasValue)
            filter &= Builders<Voucher>.Filter.Eq(x => x.Platform, platform.Value);

        return (int)await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<Voucher>> GetExpiringAsync(DateTime threshold, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Voucher>.Filter.Lte(x => x.ExpiresAt, threshold)
            & Builders<Voucher>.Filter.Eq(x => x.Status, VoucherStatus.Active);

        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Voucher>> GetByPostIdAsync(string postId, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.PostId == postId).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Voucher voucher, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(voucher, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(Voucher voucher, CancellationToken cancellationToken = default)
    {
        await _collection.ReplaceOneAsync(x => x.Id == voucher.Id, voucher, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(x => x.Id == id, cancellationToken);
    }
}
