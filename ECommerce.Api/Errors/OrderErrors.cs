namespace ECommerce.Api.Errors;

public record CartNotFoundError(int CartId)
    : Error($"Cart with id {CartId} not found");

public record CartIsEmptyError(int CartId)
    : Error($"Cart with id {CartId} is empty");

public record AddressNotFoundError(int AddressId)
    : Error($"Address with id {AddressId} not found");
