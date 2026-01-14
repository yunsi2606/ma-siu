using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UserService.Api.Grpc;
using UserService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Infrastructure
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB")
    ?? throw new InvalidOperationException("MongoDB connection string not configured");

builder.Services.AddInfrastructure(mongoConnectionString, "masiu_users");

// gRPC for inter-service communication
builder.Services.AddGrpc();

// Controllers (REST API for external clients)
builder.Services.AddControllers();

var app = builder.Build();

// Middleware
app.UseAuthentication();
app.UseAuthorization();

// Map gRPC service (internal communication)
app.MapGrpcService<UserGrpcService>();

// Map REST controllers (external API via Gateway)
app.MapControllers();

app.Run();
