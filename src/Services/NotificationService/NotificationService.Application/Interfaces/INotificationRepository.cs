using NotificationService.Domain.Entities;

namespace NotificationService.Application.Interfaces;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notification>> GetByUserIdAsync(string userId, int skip = 0, int take = 50, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(string userId, CancellationToken cancellationToken = default);
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
    Task MarkAllReadAsync(string userId, CancellationToken cancellationToken = default);
}
