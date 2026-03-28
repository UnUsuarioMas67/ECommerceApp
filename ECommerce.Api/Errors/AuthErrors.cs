namespace ECommerce.Api.Errors;

public record InvalidLoginError() : Error("Invalid login credentials");

public record InvalidAuthenticationError : Error
{
    public int? InvalidClientId { get; set; }

    public InvalidAuthenticationError(int clientId)
        : base("Invalid client authentication")
    {
        InvalidClientId = clientId;
    }

    public InvalidAuthenticationError()
        : base("Invalid client authentication")
    {
    }
}