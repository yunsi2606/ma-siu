using Microsoft.Extensions.Logging;
using Quartz;

namespace TaskService.Jobs;

/// <summary>
/// Job to expire vouchers past their expiry date.
/// Runs every hour.
/// </summary>
[DisallowConcurrentExecution]
public class VoucherExpiryJob(ILogger<VoucherExpiryJob> logger, IHttpClientFactory httpClientFactory)
    : IJob
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("VoucherService");

    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("VoucherExpiryJob started at {Time}", DateTime.UtcNow);

        try
        {
            // Call VoucherService to expire vouchers
            // This calls the MediatR ExpireVouchersCommand internally
            var response = await _httpClient.PostAsync("/api/vouchers/expire-all", null, context.CancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(context.CancellationToken);
                logger.LogInformation("VoucherExpiryJob completed. Response: {Response}", content);
            }
            else
            {
                logger.LogWarning("VoucherExpiryJob failed with status {Status}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "VoucherExpiryJob encountered an error");
        }
    }
}
