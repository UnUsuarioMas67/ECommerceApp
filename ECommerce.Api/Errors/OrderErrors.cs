namespace ECommerce.Api.Errors;

public record CartNotFoundError(int CartId)
    : Error($"Cart with id {CartId} not found");

public record CartIsEmptyError(int CartId)
    : Error($"Cart with id {CartId} is empty");

public record AddressNotFoundError(int AddressId)
    : Error($"Address with id {AddressId} not found");

public record ProductsNotEnoughStockError(ICollection<ProductsNotEnoughStockErrorItem> Products)
    : Error("One or more products have insufficient stock");
public record ProductsNotEnoughStockErrorItem(int ProductId, int QuantityRequested, int StockAvailable);