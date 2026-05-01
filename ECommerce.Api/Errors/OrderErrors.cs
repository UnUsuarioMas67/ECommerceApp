namespace ECommerce.Api.Errors;

public record CartNotFoundError(int CartId)
    : Error("Could not find cart", "cart_not_found");

public record CartIsEmptyError(int CartId)
    : Error("Cart cannot be empty", "cart_empty");

public record AddressNotFoundError(int AddressId)
    : Error("Could not find address", "address_not_found");

public record ProductsStockError(ICollection<ProductsStockErrorItem> Products)
    : Error("One or more products have insufficient stock", "insufficient_stock");
public record ProductsStockErrorItem(int ProductId, int QuantityRequested, int StockAvailable);

public record WebhookError() : Error("Webhook processing failed", "webhook_processing_failed");