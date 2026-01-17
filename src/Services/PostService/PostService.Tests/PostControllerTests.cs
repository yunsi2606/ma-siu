using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace PostService.Tests;

/// <summary>
/// Integration tests for PostService endpoints
/// </summary>
public class PostControllerTests : IClassFixture<PostServiceWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly PostServiceWebApplicationFactory _factory;

    public PostControllerTests(PostServiceWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetFeed_ReturnsOk()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/posts/feed");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetFeed_ReturnsPostsList()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/posts/feed");
        var content = await response.Content.ReadFromJsonAsync<FeedResponse>();

        // Assert
        content.Should().NotBeNull();
        content!.Posts.Should().NotBeNull();
    }

    [Fact]
    public async Task CreatePost_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange
        var request = new
        {
            title = "Test Post",
            content = "Test content",
            platform = "Shopee"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/posts", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPost_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = "000000000000000000000000";

        // Act
        var response = await _client.GetAsync($"/api/posts/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

public record FeedResponse(List<PostDto> Posts, int TotalCount, int Page, int PageSize);
public record PostDto(string Id, string Title, string Content, string Platform);
