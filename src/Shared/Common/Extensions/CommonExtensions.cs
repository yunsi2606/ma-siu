using System.Text.Json;

namespace Common.Extensions;

/// <summary>
/// String extension methods.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Checks if string is null, empty, or whitespace.
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string? value) => 
        string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Checks if string has a value (not null, empty, or whitespace).
    /// </summary>
    public static bool HasValue(this string? value) => 
        !string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Truncates string to specified length.
    /// </summary>
    public static string Truncate(this string value, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            return value;

        return value[..(maxLength - suffix.Length)] + suffix;
    }

    /// <summary>
    /// Converts string to snake_case.
    /// </summary>
    public static string ToSnakeCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return string.Concat(
            value.Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + c : c.ToString())
        ).ToLowerInvariant();
    }
}

/// <summary>
/// JSON extension methods.
/// </summary>
public static class JsonExtensions
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static string ToJson<T>(this T obj, JsonSerializerOptions? options = null) =>
        JsonSerializer.Serialize(obj, options ?? DefaultOptions);

    public static T? FromJson<T>(this string json, JsonSerializerOptions? options = null) =>
        JsonSerializer.Deserialize<T>(json, options ?? DefaultOptions);
}

/// <summary>
/// DateTime extension methods.
/// </summary>
public static class DateTimeExtensions
{
    public static long ToUnixTimeSeconds(this DateTime dateTime) =>
        new DateTimeOffset(dateTime).ToUnixTimeSeconds();

    public static long ToUnixTimeMilliseconds(this DateTime dateTime) =>
        new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();

    public static DateTime FromUnixTimeSeconds(this long timestamp) =>
        DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;

    public static DateTime FromUnixTimeMilliseconds(this long timestamp) =>
        DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime;
}
