using AuthService.Application.Interfaces;
using AuthService.Application.Services;
using AuthService.Infrastructure.Auth;
using AuthService.Infrastructure.Google;
using AuthService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace AuthService.Infrastructure;

/// <summary>
/// DI registration for Infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string mongoConnectionString,
        string mongoDatabaseName)
    {
        // MongoDB
        services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnectionString));
        services.AddScoped<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoDatabaseName);
        });

        // Repositories
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Services
        services.AddScoped<IGoogleAuthProvider, GoogleAuthProvider>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<AuthenticationService>();

        return services;
    }
}
