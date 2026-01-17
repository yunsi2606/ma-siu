using MassTransit;
using Microsoft.Extensions.Logging;
using NotificationService.Grpc;
using TaskService.Consumers.Events;

namespace TaskService.Consumers;

/// <summary>
/// Consumes VoucherCreatedEvent from RabbitMQ.
/// Triggers notification about new voucher via gRPC.
/// </summary>
public class VoucherCreatedConsumer(
    ILogger<VoucherCreatedConsumer> logger,
    NotificationGrpc.NotificationGrpcClient notificationClient)
    : IConsumer<VoucherCreatedEvent>
{
    public async Task Consume(ConsumeContext<VoucherCreatedEvent> context)
    {
        var evt = context.Message;
        logger.LogInformation("Processing VoucherCreatedEvent for VoucherId: {VoucherId}", evt.VoucherId);

        try
        {
            var request = new SendToTopicRequest
            {
                Topic = "voucher-alerts",
                Type = "NewVoucher",
                Title = $"🎁 Mã giảm giá mới từ {evt.Platform}!",
                Body = $"Code: {evt.Code} - Hết hạn: {evt.ExpiresAt:dd/MM/yyyy HH:mm}"
            };
            request.Data.Add("voucherId", evt.VoucherId);
            request.Data.Add("voucherCode", evt.Code);
            request.Data.Add("platform", evt.Platform);

            var response = await notificationClient.SendToTopicAsync(
                request,
                cancellationToken: context.CancellationToken);

            if (response.Success)
            {
                logger.LogInformation("VoucherCreatedEvent processed successfully via gRPC");
            }
            else
            {
                logger.LogWarning("VoucherCreatedEvent notification failed: {Message}", response.Message);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing VoucherCreatedEvent");
            throw;
        }
    }
}
