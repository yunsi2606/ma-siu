using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using RewardService.Application.Interfaces;
using RewardService.Application.Services;
using RewardService.Infrastructure.Repositories;

namespace RewardService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        string databaseName)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);

        services.AddSingleton(database);

        // Repositories
        services.AddScoped<IRewardRepository, RewardRepository>();
        services.AddScoped<IPointsBalanceRepository, PointsBalanceRepository>();
        services.AddScoped<IPointsTransactionRepository, PointsTransactionRepository>();
        services.AddScoped<IRedemptionRepository, RedemptionRepository>();

        // Services
        services.AddScoped<PointsService>();
        services.AddScoped<RewardCatalogService>();

        return services;
    }
}
