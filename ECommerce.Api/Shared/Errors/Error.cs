namespace ECommerce.Api.Shared.Errors;

// public record Error(ErrorType ErrorType, IDictionary<string, string[]> Details);
//
// public static class Errors
// {
//     public static Error NotFound()
//         => new Error(ErrorType.NotFound, new Dictionary<string, string[]>());
//
//     public static Error ValidationError(IDictionary<string, string[]> details)
//         => new Error(ErrorType.Validation, details);
//     
//     public static Error ValidationError(string name, string description)
//         => new Error(ErrorType.Validation, new Dictionary<string, string[]> { { name, [description] } });
//
//     public static Error AuthenticationError(string name, string description)
//         => new Error(ErrorType.Authentication, new Dictionary<string, string[]> { { name, [description] } });
//
//     public static Error ParametersError(string name, string description)
//         => new Error(ErrorType.Parameters, new Dictionary<string, string[]> { { name, [description] } });
// }
//
// public enum ErrorType
// {
//     Validation,
//     NotFound,
//     Authentication,
//     Parameters
// }

public record Error(string Message);
public record AuthenticationError() : Error("Invalid login credentials");
public record ValidationError(IDictionary<string, string[]> Details) : Error("One or more validation errors occurred");
public record NotFoundError() : Error("Could not find the requested resource");
public record ClientNotExistsError(int ClientId, object? Identifier) : Error("The specified client doesn't seem to exist");