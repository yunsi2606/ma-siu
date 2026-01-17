using System.Net.Http.Json;
using MassTransit;
using Microsoft.Extensions.Logging;
using TaskService.Consumers.Events;

namespace TaskService.Consumers;

/// <summary>
/// Consumes PostPublishedEvent from RabbitMQ.
/// Triggers notification to followers.
/// </summary>
public class PostPublishedConsumer(
    ILogger<PostPublishedConsumer> logger,
    IHttpClientFactory httpClientFactory)
    : IConsumer<PostPublishedEvent>
{
    private readonly HttpClient _notificationClient = httpClientFactory.CreateClient("NotificationService");
    private readonly HttpClient _userClient = httpClientFactory.CreateClient("UserService");

    public async Task Consume(ConsumeContext<PostPublishedEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation("Processing PostPublishedEvent for PostId: {PostId}", evt.PostId);

        try
        {
            // TODO: Get author followers from UserService (gRPC)
            // For now, send to topic
            var notifyRequest = new
            {
                Topic = "new-posts",
                Type = "NewPost",
                Title = $"📱 Bài viết mới!",
                Body = evt.Title.Length > 50 ? evt.Title[..47] + "..." : evt.Title,
                Data = new Dictionary<string, string>
                {
                    ["postId"] = evt.PostId,
                    ["authorId"] = evt.AuthorId
                }
            };

            await _notificationClient.PostAsJsonAsync(
                "/api/notifications/send-topic",
                notifyRequest,
                context.CancellationToken);

            logger.LogInformation("PostPublishedEvent processed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing PostPublishedEvent");
            throw; // Let MassTransit handle retry
        }
    }
}
