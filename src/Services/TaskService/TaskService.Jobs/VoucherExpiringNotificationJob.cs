using Microsoft.Extensions.Logging;
using NotificationService.Grpc;
using Quartz;

namespace TaskService.Jobs;

/// <summary>
/// Job to send expiring voucher notifications via gRPC.
/// Runs every 6 hours, notifies users about vouchers expiring within 24 hours.
/// </summary>
[DisallowConcurrentExecution]
public class VoucherExpiringNotificationJob(
    ILogger<VoucherExpiringNotificationJob> logger,
    IHttpClientFactory httpClientFactory,
    NotificationGrpc.NotificationGrpcClient notificationClient)
    : IJob
{
    private readonly HttpClient _voucherClient = httpClientFactory.CreateClient("VoucherService");

    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("VoucherExpiringNotificationJob started at {Time}", DateTime.UtcNow);

        try
        {
            // Get expiring vouchers (still HTTP since VoucherService has no gRPC)
            var response = await _voucherClient.GetAsync("/api/vouchers/expiring?hoursAhead=24", context.CancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Failed to get expiring vouchers: {Status}", response.StatusCode);
                return;
            }

            // Send broadcast notification via gRPC
            var request = new SendToTopicRequest
            {
                Topic = "voucher-alerts",
                Type = "VoucherExpiring",
                Title = "⚠️ Vouchers sắp hết hạn!",
                Body = "Có voucher sẽ hết hạn trong 24 giờ tới. Mở app để xem!"
            };
            request.Data.Add("action", "open_expiring");

            var grpcResponse = await notificationClient.SendToTopicAsync(
                request,
                cancellationToken: context.CancellationToken);

            if (grpcResponse.Success)
            {
                logger.LogInformation("VoucherExpiringNotificationJob completed successfully via gRPC");
            }
            else
            {
                logger.LogWarning("Notification failed: {Message}", grpcResponse.Message);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "VoucherExpiringNotificationJob encountered an error");
        }
    }
}
