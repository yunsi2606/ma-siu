using EventContracts;
using MassTransit;
using MaSiu.Grpc.Notification;
using Microsoft.Extensions.Logging;

namespace TaskService.Consumers;

/// <summary>
/// Consumes PostPublishedEvent from RabbitMQ.
/// Triggers notification to followers via gRPC.
/// </summary>
public class PostPublishedConsumer(
    ILogger<PostPublishedConsumer> logger,
    NotificationGrpc.NotificationGrpcClient notificationClient)
    : IConsumer<PostPublishedEvent>
{
    public async Task Consume(ConsumeContext<PostPublishedEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation("Processing PostPublishedEvent for PostId: {PostId}", evt.PostId);

        try
        {
            var request = new SendToTopicRequest
            {
                Topic = "new-posts",
                Type = "NewPost",
                Title = "📱 Bài viết mới!",
                Body = evt.Title.Length > 50 ? evt.Title[..47] + "..." : evt.Title
            };
            request.Data.Add("postId", evt.PostId);
            request.Data.Add("authorId", evt.AuthorId);

            var response = await notificationClient.SendToTopicAsync(
                request,
                cancellationToken: context.CancellationToken);

            if (response.Success)
            {
                logger.LogInformation("PostPublishedEvent processed successfully via gRPC");
            }
            else
            {
                logger.LogWarning("PostPublishedEvent notification failed: {Message}", response.Message);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing PostPublishedEvent");
            throw; // Let MassTransit handle retry
        }
    }
}
