namespace Common.Responses;

/// <summary>
/// Unified API response wrapper.
/// Ensures consistent response format across all services.
/// </summary>
/// <typeparam name="T">Response data type</typeparam>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public IReadOnlyList<string>? Errors { get; set; }
    public ApiMetadata? Meta { get; set; }

    protected ApiResponse() { }

    public static ApiResponse<T> Ok(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> Ok(T data, ApiMetadata meta)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Meta = meta
        };
    }

    public static ApiResponse<T> Fail(string message, IEnumerable<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors?.ToList()
        };
    }

    public static ApiResponse<T> NotFound(string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message ?? "Resource not found"
        };
    }
}

/// <summary>
/// Non-generic version for empty responses.
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Ok(string? message = null) 
    {
        return new ApiResponse { Success = true, Message = message };
    }

    public static new ApiResponse Fail(string message) 
    {
        return new ApiResponse { Success = false, Message = message };
    }
}

/// <summary>
/// Metadata for paginated responses.
/// </summary>
public class ApiMetadata
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
    public bool HasNext => Page < TotalPages;
    public bool HasPrevious => Page > 1;

    public ApiMetadata() { }

    public ApiMetadata(int page, int pageSize, int totalCount)
    {
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }
}
