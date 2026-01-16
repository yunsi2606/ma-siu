using AffiliateService.Infrastructure;
using AffiliateService.Infrastructure.Platforms;
using Common.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Shared Configuration
builder.Configuration.AddSharedConfiguration(builder.Environment.EnvironmentName);

// JWT Authentication
builder.Services.AddSharedJwtAuthentication(builder.Configuration);

// Platform Options
builder.Services.Configure<ShopeeAffiliateOptions>(
    builder.Configuration.GetSection(ShopeeAffiliateOptions.SectionName));
builder.Services.Configure<LazadaAffiliateOptions>(
    builder.Configuration.GetSection(LazadaAffiliateOptions.SectionName));
builder.Services.Configure<TikTokAffiliateOptions>(
    builder.Configuration.GetSection(TikTokAffiliateOptions.SectionName));

// Infrastructure
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB")
    ?? throw new InvalidOperationException("MongoDB connection string not configured");
var databaseName = builder.Configuration["DatabaseName"] ?? "masiu_affiliate";

builder.Services.AddInfrastructure(mongoConnectionString, databaseName);

// Controllers
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
