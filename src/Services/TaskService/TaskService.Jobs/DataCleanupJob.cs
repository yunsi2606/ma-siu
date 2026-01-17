using Microsoft.Extensions.Logging;
using Quartz;

namespace TaskService.Jobs;

/// <summary>
/// Job to clean up old data.
/// Runs daily at 3 AM.
/// </summary>
[DisallowConcurrentExecution]
public class DataCleanupJob(ILogger<DataCleanupJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("DataCleanupJob started at {Time}", DateTime.UtcNow);

        try
        {
            // TODO: Implement cleanup logic
            // - Clean old affiliate clicks (> 90 days)
            // - Clean old notifications (> 30 days, read)
            // - Clean expired Redis keys
            
            await Task.Delay(100, context.CancellationToken); // Placeholder

            logger.LogInformation("DataCleanupJob completed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DataCleanupJob encountered an error");
        }
    }
}
