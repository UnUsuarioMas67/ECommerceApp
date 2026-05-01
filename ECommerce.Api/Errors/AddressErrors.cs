namespace ECommerce.Api.Errors;

public record InvalidCountryError(string CountryCode, int? AddressId) : Error("Invalid country CCA2 code", "invalid_country_code");