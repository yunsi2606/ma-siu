using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoucherService.Application.Commands;
using VoucherService.Application.Queries;
using VoucherService.Domain.Entities;

namespace VoucherService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VouchersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<VouchersController> _logger;

    public VouchersController(IMediator mediator, ILogger<VouchersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get active vouchers with pagination.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<VouchersResult>> GetVouchers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? platform = null,
        CancellationToken cancellationToken = default)
    {
        Platform? platformFilter = null;
        if (!string.IsNullOrEmpty(platform) && Enum.TryParse<Platform>(platform, true, out var p))
            platformFilter = p;

        var result = await _mediator.Send(new GetActiveVouchersQuery(page, pageSize, platformFilter), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get voucher by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<VoucherDto>> GetById(string id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetVoucherByIdQuery(id), cancellationToken);
        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>
    /// Get vouchers expiring soon.
    /// </summary>
    [HttpGet("expiring")]
    public async Task<ActionResult<IReadOnlyList<VoucherDto>>> GetExpiring(
        [FromQuery] int hoursAhead = 24,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetExpiringSoonQuery(hoursAhead), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get vouchers by Post ID.
    /// </summary>
    [HttpGet("by-post/{postId}")]
    public async Task<ActionResult<IReadOnlyList<VoucherDto>>> GetByPost(string postId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetVouchersByPostQuery(postId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Create voucher (Admin only).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CreateVoucherResult>> Create(
        [FromBody] CreateVoucherRequest request,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<Platform>(request.Platform, true, out var platform))
            return BadRequest(new { message = "Invalid platform" });
        
        if (!Enum.TryParse<VoucherType>(request.Type, true, out var type))
            return BadRequest(new { message = "Invalid voucher type" });

        var command = new CreateVoucherCommand(
            request.Code, platform, type,
            request.StartAt, request.ExpiresAt,
            request.Description,
            request.DiscountValue, request.DiscountPercent,
            request.MinSpend, request.MaxDiscount,
            request.PostId
        );

        var result = await _mediator.Send(command, cancellationToken);
        if (!result.Success) return BadRequest(new { message = result.Error });

        _logger.LogInformation("Voucher {Code} created for {Platform}", request.Code, request.Platform);
        return CreatedAtAction(nameof(GetById), new { id = result.VoucherId }, result);
    }

    /// <summary>
    /// Deactivate voucher (Admin only).
    /// </summary>
    [HttpPost("{id}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deactivate(string id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeactivateVoucherCommand(id), cancellationToken);
        return result ? NoContent() : NotFound();
    }

    /// <summary>
    /// Track voucher usage.
    /// </summary>
    [HttpPost("{id}/track-usage")]
    public async Task<IActionResult> TrackUsage(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new TrackVoucherUsageCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Health check.
    /// </summary>
    [HttpGet("/health")]
    public IActionResult Health() => Ok(new { status = "healthy", service = "voucher-service" });
}

// Request DTOs
public record CreateVoucherRequest(
    string Code,
    string Platform,
    string Type,
    DateTime StartAt,
    DateTime ExpiresAt,
    string? Description,
    decimal? DiscountValue,
    decimal? DiscountPercent,
    decimal? MinSpend,
    decimal? MaxDiscount,
    string? PostId
);
