namespace BuildingBlocks.Abstractions;

/// <summary>
/// Unit of Work pattern for coordinating multiple repository operations.
/// Use only when transactional consistency is required across multiple entities.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
