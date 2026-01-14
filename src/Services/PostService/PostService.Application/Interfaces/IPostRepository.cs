using PostService.Domain.Entities;

namespace PostService.Application.Interfaces;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Post>> GetPublishedAsync(int skip, int take, Platform? platform = null, CancellationToken cancellationToken = default);
    Task<int> GetPublishedCountAsync(Platform? platform = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Post>> GetByAuthorAsync(string authorId, CancellationToken cancellationToken = default);
    Task AddAsync(Post post, CancellationToken cancellationToken = default);
    Task UpdateAsync(Post post, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
