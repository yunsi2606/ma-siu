using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using VoucherService.Application.Interfaces;
using VoucherService.Infrastructure.Persistence;

namespace VoucherService.Infrastructure;

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
        services.AddScoped<IVoucherRepository, VoucherRepository>();

        return services;
    }
}
