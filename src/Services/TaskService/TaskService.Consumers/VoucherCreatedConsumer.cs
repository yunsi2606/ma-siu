using System.Net.Http.Json;
using MassTransit;
using Microsoft.Extensions.Logging;
using TaskService.Consumers.Events;

namespace TaskService.Consumers;

/// <summary>
/// Consumes VoucherCreatedEvent from RabbitMQ.
/// Triggers notification about new voucher.
/// </summary>
public class VoucherCreatedConsumer(
    ILogger<VoucherCreatedConsumer> logger,
    IHttpClientFactory httpClientFactory)
    : IConsumer<VoucherCreatedEvent>
{
    private readonly HttpClient _notificationClient = httpClientFactory.CreateClient("NotificationService");

    public async Task Consume(ConsumeContext<VoucherCreatedEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation("Processing VoucherCreatedEvent for VoucherId: {VoucherId}", evt.VoucherId);

        try
        {
            var notifyRequest = new
            {
                Topic = "voucher-alerts",
                Type = "NewVoucher",
                Title = $"🎁 Mã giảm giá mới từ {evt.Platform}!",
                Body = $"Code: {evt.Code} - Hết hạn: {evt.ExpiresAt:dd/MM/yyyy HH:mm}",
                Data = new Dictionary<string, string>
                {
                    ["voucherId"] = evt.VoucherId,
                    ["voucherCode"] = evt.Code,
                    ["platform"] = evt.Platform
                }
            };

            await _notificationClient.PostAsJsonAsync(
                "/api/notifications/send-topic",
                notifyRequest,
                context.CancellationToken);

            logger.LogInformation("VoucherCreatedEvent processed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing VoucherCreatedEvent");
            throw;
        }
    }
}
