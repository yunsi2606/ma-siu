using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Application.Services;

public class UserApplicationService
{
    private readonly IUserRepository _userRepository;

    public UserApplicationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto?> GetUserByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByGoogleIdAsync(googleId, cancellationToken);
        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto> CreateOrGetUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        // Check if user already exists
        var existingUser = await _userRepository.GetByGoogleIdAsync(request.GoogleId, cancellationToken);
        if (existingUser != null)
            return MapToDto(existingUser);

        // Create new user
        var user = User.Create(
            request.GoogleId,
            request.Email,
            request.DisplayName,
            request.AvatarUrl
        );

        await _userRepository.AddAsync(user, cancellationToken);
        return MapToDto(user);
    }

    public async Task<UserDto?> UpdateProfileAsync(
        string userId,
        UpdateProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null) return null;

        user.UpdateProfile(request.DisplayName, request.AvatarUrl);
        await _userRepository.UpdateAsync(user, cancellationToken);
        return MapToDto(user);
    }

    public async Task<bool> RegisterFcmTokenAsync(
        string userId,
        RegisterFcmTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null) return false;

        user.AddFcmToken(request.Token, request.DeviceId, request.Platform);
        await _userRepository.UpdateAsync(user, cancellationToken);
        return true;
    }

    public async Task<bool> RemoveFcmTokenAsync(
        string userId,
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null) return false;

        user.RemoveFcmToken(deviceId);
        await _userRepository.UpdateAsync(user, cancellationToken);
        return true;
    }

    private static UserDto MapToDto(User user) => new(
        user.Id,
        user.Email,
        user.DisplayName,
        user.AvatarUrl,
        user.Role.ToString(),
        user.TotalPoints,
        user.PendingPoints,
        user.CreatedAt
    );
}
