using AuthService.Infrastructure;
using AuthService.Infrastructure.Auth;
using AuthService.Infrastructure.Google;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Services.Configure<GoogleAuthOptions>(
    builder.Configuration.GetSection(GoogleAuthOptions.SectionName));
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));

// Infrastructure (MongoDB, Services)
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB")
    ?? throw new InvalidOperationException("MongoDB connection string not configured");

builder.Services.AddInfrastructure(mongoConnectionString, "masiu_auth");

// Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Middleware
app.MapControllers();

app.Run();
