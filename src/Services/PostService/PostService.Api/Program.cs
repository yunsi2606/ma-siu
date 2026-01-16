using Common.Configuration;
using PostService.Application.Handlers;
using PostService.Infrastructure;
using MinIOOptions = Common.Configuration.MinIOOptions;

var builder = WebApplication.CreateBuilder(args);

// Shared Configuration (from config/ folder)
builder.Configuration.AddSharedConfiguration(builder.Environment.EnvironmentName);

// JWT Authentication (shared)
builder.Services.AddSharedJwtAuthentication(builder.Configuration);
// MediatR (CQRS)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreatePostHandler>());

// Infrastructure
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB")
    ?? throw new InvalidOperationException("MongoDB connection string not configured");
var databaseName = builder.Configuration["DatabaseName"] ?? "masiu_posts";
var storageBucket = builder.Configuration["StorageBucket"] ?? "posts";

builder.Services.AddInfrastructure(
    mongoConnectionString,
    databaseName,
    options => builder.Configuration.GetSection(MinIOOptions.SectionName).Bind(options),
    postStorageOptions => postStorageOptions.BucketName = storageBucket);

// Controllers
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

