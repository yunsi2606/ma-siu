using MassTransit;
using Quartz;
using TaskService.Consumers;
using TaskService.Jobs;

var builder = Host.CreateApplicationBuilder(args);

// =============================================================================
// Load configuration from config/ folder
// =============================================================================
var configPath = FindConfigPath();
if (!string.IsNullOrEmpty(configPath))
{
    builder.Configuration
        .AddJsonFile(Path.Combine(configPath, "appsettings.json"), optional: false, reloadOnChange: true)
        .AddJsonFile(Path.Combine(configPath, $"appsettings.{builder.Environment.EnvironmentName}.json"), optional: true);
}

// =============================================================================
// HTTP Clients for service-to-service calls
// =============================================================================
builder.Services.AddHttpClient("VoucherService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:VoucherService"] ?? "http://localhost:5104");
    client.DefaultRequestHeaders.Add("X-Internal-Api-Key", "masiu-internal-key");
});

builder.Services.AddHttpClient("NotificationService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:NotificationService"] ?? "http://localhost:5106");
    client.DefaultRequestHeaders.Add("X-Internal-Api-Key", "masiu-internal-key");
});

builder.Services.AddHttpClient("UserService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:UserService"] ?? "http://localhost:5102");
});

// =============================================================================
// Quartz Scheduler
// =============================================================================
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    // VoucherExpiryJob - Every hour
    var voucherExpiryKey = new JobKey("VoucherExpiryJob");
    q.AddJob<VoucherExpiryJob>(opts => opts.WithIdentity(voucherExpiryKey));
    q.AddTrigger(opts => opts
        .ForJob(voucherExpiryKey)
        .WithIdentity("VoucherExpiryTrigger")
        .WithCronSchedule("0 0 * * * ?")); // Every hour at minute 0

    // VoucherExpiringNotificationJob - Every 6 hours
    var expiringNotifyKey = new JobKey("VoucherExpiringNotificationJob");
    q.AddJob<VoucherExpiringNotificationJob>(opts => opts.WithIdentity(expiringNotifyKey));
    q.AddTrigger(opts => opts
        .ForJob(expiringNotifyKey)
        .WithIdentity("VoucherExpiringNotifyTrigger")
        .WithCronSchedule("0 0 */6 * * ?")); // Every 6 hours

    // DataCleanupJob - Daily at 3 AM
    var cleanupKey = new JobKey("DataCleanupJob");
    q.AddJob<DataCleanupJob>(opts => opts.WithIdentity(cleanupKey));
    q.AddTrigger(opts => opts
        .ForJob(cleanupKey)
        .WithIdentity("DataCleanupTrigger")
        .WithCronSchedule("0 0 3 * * ?")); // Daily at 3 AM
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// =============================================================================
// MassTransit + RabbitMQ
// =============================================================================
var rabbitConfig = builder.Configuration.GetSection("RabbitMQ");
builder.Services.AddMassTransit(x =>
{
    // Register consumers
    x.AddConsumer<PostPublishedConsumer>();
    x.AddConsumer<VoucherCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitConfig["Host"] ?? "localhost", ushort.Parse(rabbitConfig["Port"] ?? "5672"), rabbitConfig["VirtualHost"] ?? "/", h =>
        {
            h.Username(rabbitConfig["Username"] ?? "guest");
            h.Password(rabbitConfig["Password"] ?? "guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();

Console.WriteLine("========================================");
Console.WriteLine("  TaskService (JobOrchestrator) Started");
Console.WriteLine("========================================");
Console.WriteLine("  Quartz Jobs:");
Console.WriteLine("    - VoucherExpiryJob: Every hour");
Console.WriteLine("    - VoucherExpiringNotificationJob: Every 6 hours");
Console.WriteLine("    - DataCleanupJob: Daily at 3 AM");
Console.WriteLine("  RabbitMQ Consumers:");
Console.WriteLine("    - PostPublishedConsumer");
Console.WriteLine("    - VoucherCreatedConsumer");
Console.WriteLine("========================================");

host.Run();

static string FindConfigPath()
{
    var current = Directory.GetCurrentDirectory();
    for (int i = 0; i < 5; i++)
    {
        var configPath = Path.Combine(current, "config");
        if (Directory.Exists(configPath)) return configPath;
        var parent = Directory.GetParent(current);
        if (parent == null) break;
        current = parent.FullName;
    }
    return string.Empty;
}
