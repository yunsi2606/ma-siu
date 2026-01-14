using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Infrastructure.MongoDB;

/// <summary>
/// MongoDB configuration and service registration.
/// </summary>
public static class MongoDbConfiguration
{
    private static bool _conventionsRegistered;

    /// <summary>
    /// Registers MongoDB conventions for consistent serialization.
    /// </summary>
    public static void RegisterConventions()
    {
        if (_conventionsRegistered) return;

        var conventionPack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true),
            new EnumRepresentationConvention(BsonType.String)
        };

        ConventionRegistry.Register("MaSiuConventions", conventionPack, _ => true);

        // Register Guid serializer
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        _conventionsRegistered = true;
    }

    /// <summary>
    /// Adds MongoDB services to the DI container.
    /// </summary>
    public static IServiceCollection AddMongoDb(
        this IServiceCollection services,
        string connectionString,
        string databaseName)
    {
        RegisterConventions();

        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
        
        services.AddScoped<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(databaseName);
        });

        return services;
    }
}
