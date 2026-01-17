using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.MongoDb;
using Testcontainers.Redis;

namespace PostService.Tests;

/// <summary>
/// Custom WebApplicationFactory for PostService integration tests
/// </summary>
public class PostServiceWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder()
        .WithImage("mongo:7.0")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:7.4-alpine")
        .Build();

    public string MongoConnectionString => _mongoContainer.GetConnectionString();
    public string RedisConnectionString => _redisContainer.GetConnectionString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Override connection strings for tests
        });

        builder.UseEnvironment("Testing");
    }

    public async Task InitializeAsync()
    {
        await Task.WhenAll(
            _mongoContainer.StartAsync(),
            _redisContainer.StartAsync()
        );
    }

    public new async Task DisposeAsync()
    {
        await Task.WhenAll(
            _mongoContainer.DisposeAsync().AsTask(),
            _redisContainer.DisposeAsync().AsTask()
        );
        await base.DisposeAsync();
    }
}
