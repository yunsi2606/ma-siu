namespace Common.Configuration;

/// <summary>
/// Shared JWT configuration options.
/// Used across all services for token validation.
/// </summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";
    
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 15;
    public int RefreshTokenExpirationDays { get; set; } = 7;
}

/// <summary>
/// Shared MongoDB configuration options.
/// </summary>
public class MongoDbOptions
{
    public const string SectionName = "ConnectionStrings";
    
    public string MongoDB { get; set; } = string.Empty;
}

/// <summary>
/// Shared Redis configuration options.
/// </summary>
public class RedisOptions
{
    public const string SectionName = "ConnectionStrings";
    
    public string Redis { get; set; } = string.Empty;
}

/// <summary>
/// Shared RabbitMQ configuration options.
/// </summary>
public class RabbitMQOptions
{
    public const string SectionName = "RabbitMQ";
    
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string VirtualHost { get; set; } = "/";
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
}

/// <summary>
/// Shared MinIO configuration options.
/// </summary>
public class MinIOOptions
{
    public const string SectionName = "MinIO";
    
    public string Endpoint { get; set; } = "localhost:9000";
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public bool UseSSL { get; set; }
    public string PublicEndpoint { get; set; } = string.Empty;
}

/// <summary>
/// Shared Google Auth configuration options.
/// </summary>
public class GoogleAuthOptions
{
    public const string SectionName = "Google";
    
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}

/// <summary>
/// Shared Firebase configuration options.
/// </summary>
public class FirebaseOptions
{
    public const string SectionName = "Firebase";
    
    public string ProjectId { get; set; } = string.Empty;
    public string CredentialsPath { get; set; } = string.Empty;
}
