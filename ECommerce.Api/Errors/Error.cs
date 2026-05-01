using ECommerce.Api.DTOs.Error;

namespace ECommerce.Api.Errors;

// TODO - Add the ErrorId Property to Error record

public record Error(string Message, string ErrorType)
{
    public static Error None() => new Error("None", "none");

    public virtual ErrorDto ToDto() => new()
    {
        ErrorType = ErrorType,
        Message = Message,
        Details = new Dictionary<string, object?>()
    };
}
public record ValidationError(IDictionary<string, string[]> Details) : Error("One or more validation errors occurred", "validation_error")
{
    public override ErrorDto ToDto() => new()
    {
        ErrorType = ErrorType,
        Message = Message,
        Details = new Dictionary<string, object?>
        {
            ["details"] = Details
        }
    };
}
public record NotFoundError() : Error("Could not find the requested resource", "not_found")
{
    public override ErrorDto ToDto() => new()
    {
        ErrorType = ErrorType,
        Message = Message,
        Details = new Dictionary<string, object?>()
    };
}
public record ClientNotExistsError(int ClientId, object? Identifier) : Error("The specified client doesn't seem to exist", "client_not_found")
{
    public override ErrorDto ToDto() => new()
    {
        ErrorType = ErrorType,
        Message = Message,
        Details = new Dictionary<string, object?>
        {
            ["clientId"] = ClientId,
            ["identifier"] = Identifier
        }
    };
}