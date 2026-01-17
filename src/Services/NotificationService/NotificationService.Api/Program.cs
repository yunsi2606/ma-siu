using Common.Configuration;
using NotificationService.Infrastructure;
using FirebaseOptions = Common.Configuration.FirebaseOptions;

var builder = WebApplication.CreateBuilder(args);

// Shared Configuration (from config/ folder)
builder.Configuration.AddSharedConfiguration(builder.Environment.EnvironmentName);

// JWT Authentication (shared)
builder.Services.AddSharedJwtAuthentication(builder.Configuration);

// Firebase Options
builder.Services.Configure<FirebaseOptions>(
    builder.Configuration.GetSection(FirebaseOptions.SectionName));

// Infrastructure
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB")
    ?? throw new InvalidOperationException("MongoDB connection string not configured");
var redisConnectionString = builder.Configuration.GetConnectionString("Redis")
    ?? throw new InvalidOperationException("Redis connection string not configured");
var databaseName = builder.Configuration["DatabaseName"] ?? "masiu_notifications";

builder.Services.AddInfrastructure(mongoConnectionString, databaseName, redisConnectionString);

// Controllers
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
