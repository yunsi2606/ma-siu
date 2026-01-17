using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace VoucherService.Tests;

/// <summary>
/// Integration tests for VoucherService endpoints
/// </summary>
public class VoucherControllerTests : IClassFixture<VoucherServiceWebApplicationFactory>
{
    private readonly HttpClient _client;

    public VoucherControllerTests(VoucherServiceWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetActiveVouchers_ReturnsOk()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/vouchers/active");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetActiveVouchers_ReturnsVouchersList()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/vouchers/active");
        var content = await response.Content.ReadFromJsonAsync<VouchersResponse>();

        // Assert
        content.Should().NotBeNull();
        content!.Vouchers.Should().NotBeNull();
    }

    [Fact]
    public async Task GetVoucher_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = "000000000000000000000000";

        // Act
        var response = await _client.GetAsync($"/api/vouchers/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetVouchersByPlatform_ReturnsOk()
    {
        // Arrange
        var platform = "Shopee";

        // Act
        var response = await _client.GetAsync($"/api/vouchers/platform/{platform}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateVoucher_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        var request = new
        {
            code = "TEST50K",
            platform = "Shopee",
            discountAmount = 50000,
            expiresAt = DateTime.UtcNow.AddDays(7)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/vouchers", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

public record VouchersResponse(List<VoucherDto> Vouchers);
public record VoucherDto(string Id, string Code, string Platform, decimal DiscountAmount);
