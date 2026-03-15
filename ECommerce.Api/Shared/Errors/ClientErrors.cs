namespace ECommerce.Api.Shared.Errors;

public record DuplicateEmailError(string Email, int? ClientId) 
    : Error("This email address is already in use");
public record DuplicatePhoneNumberError(string PhoneNumber, int? ClientId) 
    : Error("This phone number is already in use");