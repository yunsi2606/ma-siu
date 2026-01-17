using Grpc.Core;
using NotificationService.Application.Services;
using NotificationService.Domain.Entities;
using NotificationService.Grpc;

namespace NotificationService.Api.Services;

/// <summary>
/// gRPC service for inter-service notification calls.
/// </summary>
public class NotificationGrpcService(
    NotificationSenderService senderService,
    ILogger<NotificationGrpcService> logger)
    : NotificationGrpc.NotificationGrpcBase
{
    /// <summary>
    /// Send notification to specific user.
    /// </summary>
    public override async Task<SendNotificationResponse> SendNotification(
        SendNotificationRequest request,
        ServerCallContext context)
    {
        logger.LogInformation("gRPC SendNotification to user {UserId}, type: {Type}", 
            request.UserId, request.Type);

        try
        {
            var success = await senderService.SendAsync(
                request.UserId,
                request.FcmTokens.ToList(),
                Enum.Parse<NotificationType>(request.Type),
                request.Title,
                request.Body,
                string.IsNullOrEmpty(request.ImageUrl) ? null : request.ImageUrl,
                request.Data.Count > 0 ? new Dictionary<string, string>(request.Data) : null,
                string.IsNullOrEmpty(request.DeduplicationKey) ? null : request.DeduplicationKey,
                context.CancellationToken);

            return new SendNotificationResponse
            {
                Success = success,
                Message = success ? "Notification sent" : "Notification deduplicated or failed"
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending notification via gRPC");
            return new SendNotificationResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    /// <summary>
    /// Send notification to topic (broadcast).
    /// </summary>
    public override async Task<SendToTopicResponse> SendToTopic(
        SendToTopicRequest request,
        ServerCallContext context)
    {
        logger.LogInformation("gRPC SendToTopic: {Topic}, type: {Type}", 
            request.Topic, request.Type);

        try
        {
            var success = await senderService.SendToTopicAsync(
                request.Topic,
                Enum.Parse<NotificationType>(request.Type),
                request.Title,
                request.Body,
                string.IsNullOrEmpty(request.ImageUrl) ? null : request.ImageUrl,
                request.Data.Count > 0 ? new Dictionary<string, string>(request.Data) : null,
                context.CancellationToken);

            return new SendToTopicResponse
            {
                Success = success,
                Message = success ? "Topic notification sent" : "Topic notification failed"
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending topic notification via gRPC");
            return new SendToTopicResponse
            {
                Success = false,
                Message = ex.Message
            };
        }
    }
}
