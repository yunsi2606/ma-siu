using Common.Configuration;
using UserService.Api.Grpc;
using UserService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Shared Configuration (from config/ folder)
builder.Configuration.AddSharedConfiguration(builder.Environment.EnvironmentName);

// JWT Authentication (shared)
builder.Services.AddSharedJwtAuthentication(builder.Configuration);

// Infrastructure
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB")
    ?? throw new InvalidOperationException("MongoDB connection string not configured");
var databaseName = builder.Configuration["DatabaseName"] ?? "masiu_users";

builder.Services.AddInfrastructure(mongoConnectionString, databaseName);

// gRPC for inter-service communication
builder.Services.AddGrpc();

// Controllers (REST API for external clients)
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Map gRPC service (internal communication)
app.MapGrpcService<UserGrpcService>();

// Map REST controllers (external API via Gateway)
app.MapControllers();

app.Run();

