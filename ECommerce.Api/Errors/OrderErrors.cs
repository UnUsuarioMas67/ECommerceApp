namespace ECommerce.Api.Errors;

public record CartNotFoundError(int CartId)
    : Error("Could not find cart");

public record CartIsEmptyError(int CartId)
    : Error("Cart cannot be empty");

public record AddressNotFoundError(int AddressId)
    : Error("Could not find address");

public record ProductsStockError(ICollection<ProductsStockErrorItem> Products)
    : Error("One or more products have insufficient stock");
public record ProductsStockErrorItem(int ProductId, int QuantityRequested, int StockAvailable);

public record WebhookError() : Error("Webhook processing failed");