using Common.Configuration;
using RewardService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Shared Configuration (from config/ folder)
builder.Configuration.AddSharedConfiguration(builder.Environment.EnvironmentName);

// JWT Authentication (shared)
builder.Services.AddSharedJwtAuthentication(builder.Configuration);

// Infrastructure
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB")
    ?? throw new InvalidOperationException("MongoDB connection string not configured");
var databaseName = builder.Configuration["DatabaseName"] ?? "masiu_rewards";

builder.Services.AddInfrastructure(mongoConnectionString, databaseName);

// Controllers
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
