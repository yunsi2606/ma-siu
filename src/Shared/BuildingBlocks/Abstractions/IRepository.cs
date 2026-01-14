using System.Linq.Expressions;

namespace BuildingBlocks.Abstractions;

/// <summary>
/// Generic repository interface for CRUD operations.
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    
    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
    
    Task<T?> FindOneAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
    
    Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
    
    Task<long> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default);
    
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
