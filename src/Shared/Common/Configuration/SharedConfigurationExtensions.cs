using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Common.Configuration;

/// <summary>
/// Extension methods for shared configuration setup.
/// </summary>
public static class SharedConfigurationExtensions
{
    /// <summary>
    /// Adds shared configuration from the config/ directory.
    /// </summary>
    public static IConfigurationBuilder AddSharedConfiguration(
        this IConfigurationBuilder builder,
        string environment)
    {
        var basePath = FindConfigPath();
        
        if (!string.IsNullOrEmpty(basePath))
        {
            builder
                .AddJsonFile(Path.Combine(basePath, "appsettings.json"), optional: false, reloadOnChange: true)
                .AddJsonFile(Path.Combine(basePath, $"appsettings.{environment}.json"), optional: true, reloadOnChange: true);
        }
        
        return builder;
    }

    /// <summary>
    /// Adds JWT Bearer authentication with shared configuration.
    /// </summary>
    public static IServiceCollection AddSharedJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException("JWT configuration not found");

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = "role"
                };
            });

        services.AddAuthorization();

        return services;
    }

    /// <summary>
    /// Find the config/ directory by walking up from current directory.
    /// </summary>
    private static string FindConfigPath()
    {
        var current = Directory.GetCurrentDirectory();
        
        // Walk up to find config/ folder
        for (int i = 0; i < 5; i++)
        {
            var configPath = Path.Combine(current, "config");
            if (Directory.Exists(configPath))
                return configPath;
            
            var parent = Directory.GetParent(current);
            if (parent == null) break;
            current = parent.FullName;
        }
        
        return string.Empty;
    }
}
