namespace ECommerce.Api.Errors;

public record InvalidLoginError() : Error("Invalid login credentials");

public record InvalidAuthenticationError : Error
{
    public int? InvalidClientId { get; set; }

    public InvalidAuthenticationError(int clientId)
        : base("User does not have permission to access this resource")
    {
        InvalidClientId = clientId;
    }

    public InvalidAuthenticationError()
        : base("Missing user identifier")
    {
    }
}