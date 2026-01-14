namespace PostService.Application.Services;

/// <summary>
/// Calculates voucher remaining percentage based on time decay.
/// UX-driven approach - no real inventory data from platforms.
/// </summary>
public static class VoucherRemainingCalculator
{
    /// <summary>
    /// Calculate remaining percentage based on time.
    /// </summary>
    /// <param name="createdAt">When the voucher was created/started</param>
    /// <param name="expiresAt">When the voucher expires</param>
    /// <returns>Remaining percentage (0-100)</returns>
    public static int CalculateRemainingPercent(DateTime createdAt, DateTime expiresAt)
    {
        var now = DateTime.UtcNow;
        
        if (now >= expiresAt)
            return 0;
            
        if (now <= createdAt)
            return 100;

        var totalDuration = (expiresAt - createdAt).TotalSeconds;
        var timeRemaining = (expiresAt - now).TotalSeconds;

        if (totalDuration <= 0)
            return 0;

        return (int)((timeRemaining / totalDuration) * 100);
    }

    /// <summary>
    /// Get status color based on remaining percentage.
    /// </summary>
    public static VoucherStatus GetStatus(int remainingPercent) => remainingPercent switch
    {
        >= 50 => VoucherStatus.Green,   // Plenty of time
        >= 20 => VoucherStatus.Yellow,  // Running low
        _ => VoucherStatus.Red          // Almost expired
    };
}

public enum VoucherStatus
{
    Green = 1,   // >= 50%
    Yellow = 2,  // 20-49%
    Red = 3      // < 20%
}
