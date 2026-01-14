using System.Text;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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

// Rate Limiting (public HTTP endpoints only)
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

// YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// CORS for Admin Web App
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAdminApp", policy =>
    {
        policy.WithOrigins(
                "https://admin-sale.nhatcuong.io.vn",
                "http://localhost:3000" // Dev
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Middleware Pipeline
app.UseIpRateLimiting();
app.UseCors("AllowAdminApp");
app.UseAuthentication();
app.UseAuthorization();

// Affiliate Redirect Endpoint: /go/{postId}
app.MapGet("/go/{postId}", async (string postId, HttpContext context) =>
{
    // TODO: Forward to AffiliateService to get redirect URL
    // For now, redirect to affiliate cluster
    var affiliateServiceUrl = builder.Configuration["ReverseProxy:Clusters:affiliate-cluster:Destinations:destination1:Address"];
    return Results.Redirect($"{affiliateServiceUrl}redirect/{postId}");
}).AllowAnonymous();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "api-gateway" }));

// YARP Routes
app.MapReverseProxy();

app.Run();
