namespace ECommerce.Api.Errors;

public record DuplicateEmailError(string Email, int? ClientId) 
    : Error("This email address is already in use", "email_already_used");
public record DuplicatePhoneNumberError(string PhoneNumber, int? ClientId) 
    : Error("This phone number is already in use", "phone_already_used");