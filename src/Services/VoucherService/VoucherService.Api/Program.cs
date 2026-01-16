using Common.Configuration;
using VoucherService.Application.Handlers;
using VoucherService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Shared Configuration (from config/ folder)
builder.Configuration.AddSharedConfiguration(builder.Environment.EnvironmentName);

// JWT Authentication (shared)
builder.Services.AddSharedJwtAuthentication(builder.Configuration);
// MediatR (CQRS)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateVoucherHandler>());

// Infrastructure
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB")
    ?? throw new InvalidOperationException("MongoDB connection string not configured");
var databaseName = builder.Configuration["DatabaseName"] ?? "masiu_vouchers";

builder.Services.AddInfrastructure(mongoConnectionString, databaseName);

// Controllers
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

