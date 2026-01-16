using AffiliateService.Application.Interfaces;
using AffiliateService.Application.Services;
using AffiliateService.Domain.Strategies;
using AffiliateService.Infrastructure.Persistence;
using AffiliateService.Infrastructure.Platforms;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace AffiliateService.Infrastructure;

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
        services.AddScoped<IAffiliateClickRepository, AffiliateClickRepository>();

        // Platform strategies (Strategy Pattern)
        services.AddScoped<IAffiliatePlatform, ShopeePlatform>();
        services.AddScoped<IAffiliatePlatform, LazadaPlatform>();
        services.AddScoped<IAffiliatePlatform, TikTokShopPlatform>();

        // Affiliate Link Service (Facade)
        services.AddScoped<AffiliateLinkService>();

        return services;
    }
}
