using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace AuthService.Tests;

/// <summary>
/// Integration tests for AuthService endpoints
/// </summary>
public class AuthControllerTests : IClassFixture<AuthServiceWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly AuthServiceWebApplicationFactory _factory;

    public AuthControllerTests(AuthServiceWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GoogleAuth_WithInvalidToken_ReturnsBadRequest()
    {
        // Arrange
        var request = new { idToken = "invalid_token" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/google", request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshToken_WithInvalidToken_ReturnsUnauthorized()
    {
        // Arrange
        var request = new { refreshToken = "invalid_refresh_token" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/refresh", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Revoke_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        var request = new { refreshToken = "some_token" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/revoke", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
