using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.RabbitMQ;

/// <summary>
/// RabbitMQ configuration options.
/// </summary>
public class RabbitMQOptions
{
    public const string SectionName = "RabbitMQ";
    
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string VirtualHost { get; set; } = "/";
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
}

/// <summary>
/// RabbitMQ/MassTransit configuration and service registration.
/// </summary>
public static class RabbitMQConfiguration
{
    /// <summary>
    /// Adds MassTransit with RabbitMQ to the DI container.
    /// </summary>
    public static IServiceCollection AddRabbitMQMessaging(
        this IServiceCollection services,
        Action<RabbitMQOptions> configureOptions,
        Action<IBusRegistrationConfigurator>? configureConsumers = null)
    {
        var options = new RabbitMQOptions();
        configureOptions(options);

        services.AddMassTransit(x =>
        {
            // Register consumers if provided
            configureConsumers?.Invoke(x);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(options.Host, (ushort)options.Port, options.VirtualHost, h =>
                {
                    h.Username(options.Username);
                    h.Password(options.Password);
                });

                // Configure endpoints for consumers
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    /// <summary>
    /// Adds MassTransit without consumers (for publishers only).
    /// </summary>
    public static IServiceCollection AddRabbitMQPublisher(
        this IServiceCollection services,
        Action<RabbitMQOptions> configureOptions)
    {
        return AddRabbitMQMessaging(services, configureOptions);
    }
}
