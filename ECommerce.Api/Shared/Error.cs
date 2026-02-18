namespace ECommerce.Api.Shared;

public record Error(ErrorType ErrorType, IDictionary<string, string[]> Details);

public static class Errors
{
    public static Error NotFound()
        => new Error(ErrorType.NotFound, new Dictionary<string, string[]>());

    public static Error ValidationError(IDictionary<string, string[]> details)
        => new Error(ErrorType.Validation, details); 
    
    public static Error AuthenticationError(string name, string description)
        => new Error(ErrorType.Authentication, new Dictionary<string, string[]> { { name, [description] } });
}

public enum ErrorType
{
    Validation,
    NotFound,
    Authentication
}