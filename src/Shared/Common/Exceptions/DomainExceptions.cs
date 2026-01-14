namespace Common.Exceptions;

/// <summary>
/// Base exception for domain-specific errors.
/// </summary>
public class DomainException : Exception
{
    public string Code { get; }

    public DomainException(string message, string code = "DOMAIN_ERROR") 
        : base(message)
    {
        Code = code;
    }
}

/// <summary>
/// Thrown when a requested entity is not found.
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, string id)
        : base($"{entityName} with id '{id}' was not found.", "NOT_FOUND")
    {
    }
}

/// <summary>
/// Thrown when validation fails.
/// </summary>
public class ValidationException : DomainException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.", "VALIDATION_ERROR")
    {
        Errors = errors;
    }

    public ValidationException(string field, string message)
        : base(message, "VALIDATION_ERROR")
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { message } }
        };
    }
}

/// <summary>
/// Thrown when user is not authorized for an operation.
/// </summary>
public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message = "Unauthorized access")
        : base(message, "UNAUTHORIZED")
    {
    }
}

/// <summary>
/// Thrown when user doesn't have permission for an operation.
/// </summary>
public class ForbiddenException : DomainException
{
    public ForbiddenException(string message = "Access forbidden")
        : base(message, "FORBIDDEN")
    {
    }
}
