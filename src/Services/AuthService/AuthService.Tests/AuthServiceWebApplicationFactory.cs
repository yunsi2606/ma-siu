using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MongoDb;

namespace AuthService.Tests;

/// <summary>
/// Custom WebApplicationFactory for AuthService integration tests
/// Uses Testcontainers for MongoDB
/// </summary>
public class AuthServiceWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer = new MongoDbBuilder()
        .WithImage("mongo:7.0")
        .Build();

    public string MongoConnectionString => _mongoContainer.GetConnectionString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Override MongoDB connection string for tests
            // The actual service should read from configuration
        });

        builder.UseEnvironment("Testing");
    }

    public async Task InitializeAsync()
    {
        await _mongoContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _mongoContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}
