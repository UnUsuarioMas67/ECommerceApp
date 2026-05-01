namespace ECommerce.Api.Errors;

// TODO - Add the ErrorId Property to Error record

public record Error(string Message, string? ErrorType = null)
{
    public static Error None() => new Error("None", "none");
}
public record ValidationError(IDictionary<string, string[]> Details) : Error("One or more validation errors occurred", "validation_error");
public record NotFoundError() : Error("Could not find the requested resource", "not_found");
public record ClientNotExistsError(int ClientId, object? Identifier) : Error("The specified client doesn't seem to exist", "client_not_found");