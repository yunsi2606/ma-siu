namespace TaskService.Consumers.Events;

/// <summary>
/// Event contracts for RabbitMQ messaging.
/// These should match EventContracts in Shared.
/// </summary>

public record PostCreatedEvent(
    string PostId,
    string AuthorId,
    string Title,
    string Platform,
    DateTime CreatedAt
);

public record PostPublishedEvent(
    string PostId,
    string AuthorId,
    string Title,
    string? AffiliateUrl,
    DateTime PublishedAt
);

public record VoucherCreatedEvent(
    string VoucherId,
    string Code,
    string Platform,
    DateTime ExpiresAt
);

public record UserRegisteredEvent(
    string UserId,
    string Email,
    string DisplayName,
    DateTime RegisteredAt
);
