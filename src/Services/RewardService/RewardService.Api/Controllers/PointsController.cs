using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RewardService.Application.Services;
using RewardService.Domain.Entities;

namespace RewardService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PointsController(PointsService pointsService) : ControllerBase
{
    /// <summary>
    /// Get user's points balance.
    /// </summary>
    [HttpGet("balance")]
    public async Task<ActionResult<PointsBalanceDto>> GetBalance(CancellationToken ct)
    {
        var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var balance = await pointsService.GetBalanceAsync(userId, ct);
        return Ok(MapToDto(balance));
    }

    /// <summary>
    /// Get user's transaction history.
    /// </summary>
    [HttpGet("transactions")]
    public async Task<ActionResult<TransactionsResponse>> GetTransactions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var transactions = await pointsService.GetTransactionsAsync(userId, page, pageSize, ct);
        return Ok(new TransactionsResponse(transactions.Select(MapTransactionToDto).ToList()));
    }

    /// <summary>
    /// Internal API: Add points to user.
    /// </summary>
    [HttpPost("add")]
    [AllowAnonymous]
    public async Task<IActionResult> AddPoints([FromBody] AddPointsRequest request, CancellationToken ct)
    {
        // Validate internal API key
        var apiKey = Request.Headers["X-Internal-Api-Key"].FirstOrDefault();
        if (apiKey != "masiu-internal-key")
            return Unauthorized();

        var balance = await pointsService.AddPointsAsync(
            request.UserId,
            request.Amount,
            request.Reason,
            request.ReferenceId,
            request.IsPending,
            ct);

        return Ok(MapToDto(balance));
    }

    private static PointsBalanceDto MapToDto(PointsBalance b) => new(
        b.UserId, b.TotalPoints, b.AvailablePoints, b.PendingPoints, b.LifetimePoints, b.LastUpdated
    );

    private static PointsTransactionDto MapTransactionToDto(PointsTransaction t) => new(
        t.Id, t.Amount, t.Type.ToString(), t.Reason, t.ReferenceId, t.Status.ToString(), t.CreatedAt
    );
}

// DTOs
public record PointsBalanceDto(string UserId, int TotalPoints, int AvailablePoints, int PendingPoints, int LifetimePoints, DateTime LastUpdated);
public record TransactionsResponse(List<PointsTransactionDto> Transactions);
public record PointsTransactionDto(string Id, int Amount, string Type, string Reason, string? ReferenceId, string Status, DateTime CreatedAt);
public record AddPointsRequest(string UserId, int Amount, string Reason, string? ReferenceId = null, bool IsPending = false);
