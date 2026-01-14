namespace Common;

/// <summary>
/// Represents the result of an operation that can either succeed or fail.
/// Use this instead of throwing exceptions for expected failures.
/// </summary>
/// <typeparam name="T">Type of the value on success</typeparam>
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public string? Error { get; }
    public string? ErrorCode { get; }

    private Result(bool isSuccess, T? value, string? error, string? errorCode)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        ErrorCode = errorCode;
    }

    public static Result<T> Success(T value) => new(true, value, null, null);
    
    public static Result<T> Failure(string error, string? errorCode = null) => 
        new(false, default, error, errorCode);

    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<string, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value!) : onFailure(Error!);
    }
}

/// <summary>
/// Result without a value, for operations that only signal success/failure.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }
    public string? ErrorCode { get; }

    private Result(bool isSuccess, string? error, string? errorCode)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
    }

    public static Result Success() => new(true, null, null);
    
    public static Result Failure(string error, string? errorCode = null) => 
        new(false, error, errorCode);
}
