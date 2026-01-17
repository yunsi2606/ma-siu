using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Quartz;

namespace TaskService.Jobs;

/// <summary>
/// Job to send expiring voucher notifications.
/// Runs every 6 hours, notifies users about vouchers expiring within 24 hours.
/// </summary>
[DisallowConcurrentExecution]
public class VoucherExpiringNotificationJob(
    ILogger<VoucherExpiringNotificationJob> logger,
    IHttpClientFactory httpClientFactory)
    : IJob
{
    private readonly HttpClient _voucherClient = httpClientFactory.CreateClient("VoucherService");
    private readonly HttpClient _notificationClient = httpClientFactory.CreateClient("NotificationService");

    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("VoucherExpiringNotificationJob started at {Time}", DateTime.UtcNow);

        try
        {
            // Get expiring vouchers
            var response = await _voucherClient.GetAsync("/api/vouchers/expiring?hoursAhead=24", context.CancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Failed to get expiring vouchers: {Status}", response.StatusCode);
                return;
            }

            // Send broadcast notification for expiring vouchers
            var notifyRequest = new
            {
                Topic = "voucher-alerts",
                Type = "VoucherExpiring",
                Title = "⚠️ Vouchers sắp hết hạn!",
                Body = "Có voucher sẽ hết hạn trong 24 giờ tới. Mở app để xem!",
                Data = new Dictionary<string, string>
                {
                    ["action"] = "open_expiring"
                }
            };

            var notifyResponse = await _notificationClient.PostAsJsonAsync("/api/notifications/send-topic", notifyRequest, context.CancellationToken);

            if (notifyResponse.IsSuccessStatusCode)
            {
                logger.LogInformation("VoucherExpiringNotificationJob completed successfully");
            }
            else
            {
                logger.LogWarning("Failed to send notification: {Status}", notifyResponse.StatusCode);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "VoucherExpiringNotificationJob encountered an error");
        }
    }
}
