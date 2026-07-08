using ECommerce.Api.DTOs.Error;

namespace ECommerce.Api.Errors;

public record DuplicateEmailError(string Email, int? ClientId) 
    : Error("This email address is already in use", "email_already_used")
{
    public override ErrorDto ToDto() => new()
    {
        ErrorType = ErrorType,
        Message = Message,
        Details = new Dictionary<string, object?>
        {
            ["email"] = Email,
            ["clientId"] = ClientId
        }
    };
}
public record DuplicatePhoneNumberError(string PhoneNumber, int? ClientId) 
    : Error("This phone number is already in use", "phone_already_used")
{
    public override ErrorDto ToDto() => new()
    {
        ErrorType = ErrorType,
        Message = Message,
        Details = new Dictionary<string, object?>
        {
            ["phoneNumber"] = PhoneNumber,
            ["clientId"] = ClientId
        }
    };
}

public record IncorrectPasswordError() : Error("Old password is incorrect", "password_update");