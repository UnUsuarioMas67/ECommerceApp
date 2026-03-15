namespace ECommerce.Api.Shared.Errors;

public record InvalidCountryError(string CountryCode, int? AddressId) : Error("Invalid country CCA2 code");