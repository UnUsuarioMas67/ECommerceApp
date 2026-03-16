namespace ECommerce.Api.Shared.Errors;

public record Error(string Message);
public record AuthenticationError() : Error("Invalid login credentials");
public record ValidationError(IDictionary<string, string[]> Details) : Error("One or more validation errors occurred");
public record NotFoundError() : Error("Could not find the requested resource");
public record ClientNotExistsError(int ClientId, object? Identifier) : Error("The specified client doesn't seem to exist");