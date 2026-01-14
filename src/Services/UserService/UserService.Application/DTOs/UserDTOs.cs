namespace UserService.Application.DTOs;

public record UserDto(
    string Id,
    string Email,
    string? DisplayName,
    string? AvatarUrl,
    string Role,
    int TotalPoints,
    int PendingPoints,
    DateTime CreatedAt
);

public record UpdateProfileRequest(
    string? DisplayName,
    string? AvatarUrl
);

public record RegisterFcmTokenRequest(
    string Token,
    string DeviceId,
    string? Platform
);

public record CreateUserRequest(
    string GoogleId,
    string Email,
    string? DisplayName,
    string? AvatarUrl
);
