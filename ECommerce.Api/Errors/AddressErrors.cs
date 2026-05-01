using ECommerce.Api.DTOs.Error;

namespace ECommerce.Api.Errors;

public record InvalidCountryError(string CountryCode, int? AddressId) : Error("Invalid country CCA2 code", "invalid_country_code")
{
    public new ErrorDto ToDto() => new()
    {
        ErrorType = ErrorType,
        Message = Message,
        Details = new Dictionary<string, object?>
        {
            ["countryCode"] = CountryCode,
            ["addressId"] = AddressId
        }
    };
}