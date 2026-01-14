using Grpc.Core;
using UserService.Application.Interfaces;
using UserService.Grpc;

namespace UserService.Api.Grpc;

/// <summary>
/// gRPC service implementation for inter-service communication.
/// Used by other services (PostService, NotificationService) to get user data.
/// </summary>
public class UserGrpcService : UserGrpc.UserGrpcBase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserGrpcService> _logger;

    public UserGrpcService(
        IUserRepository userRepository,
        ILogger<UserGrpcService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public override async Task<UserResponse> GetUser(GetUserRequest request, ServerCallContext context)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, context.CancellationToken);
        return MapToResponse(user);
    }

    public override async Task<UserResponse> GetUserByGoogleId(GetUserByGoogleIdRequest request, ServerCallContext context)
    {
        var user = await _userRepository.GetByGoogleIdAsync(request.GoogleId, context.CancellationToken);
        return MapToResponse(user);
    }

    public override async Task<GetUsersResponse> GetUsers(GetUsersRequest request, ServerCallContext context)
    {
        var response = new GetUsersResponse();
        
        foreach (var userId in request.UserIds)
        {
            var user = await _userRepository.GetByIdAsync(userId, context.CancellationToken);
            response.Users.Add(MapToResponse(user));
        }
        
        return response;
    }

    public override async Task<GetFcmTokensResponse> GetFcmTokens(GetFcmTokensRequest request, ServerCallContext context)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, context.CancellationToken);
        var response = new GetFcmTokensResponse();
        
        if (user != null)
        {
            foreach (var token in user.FcmTokens)
            {
                response.Tokens.Add(new FcmTokenInfo
                {
                    UserId = user.Id,
                    Token = token.Token,
                    DeviceId = token.DeviceId,
                    Platform = token.Platform ?? ""
                });
            }
        }
        
        return response;
    }

    public override async Task<GetFcmTokensResponse> GetAllActiveFcmTokens(GetAllActiveFcmTokensRequest request, ServerCallContext context)
    {
        var users = await _userRepository.GetAllActiveAsync(context.CancellationToken);
        var response = new GetFcmTokensResponse();
        
        foreach (var user in users)
        {
            foreach (var token in user.FcmTokens)
            {
                response.Tokens.Add(new FcmTokenInfo
                {
                    UserId = user.Id,
                    Token = token.Token,
                    DeviceId = token.DeviceId,
                    Platform = token.Platform ?? ""
                });
            }
        }

        _logger.LogInformation("Returning {Count} FCM tokens for broadcast", response.Tokens.Count);
        return response;
    }

    private static UserResponse MapToResponse(Domain.Entities.User? user)
    {
        if (user == null)
        {
            return new UserResponse { Found = false };
        }

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName ?? "",
            AvatarUrl = user.AvatarUrl ?? "",
            Role = user.Role.ToString(),
            TotalPoints = user.TotalPoints,
            Found = true
        };
    }
}
