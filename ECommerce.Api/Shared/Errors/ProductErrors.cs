namespace ECommerce.Api.Shared.Errors;

public record DuplicateProductSkuError(string Sku, int? ProductId)
    : Error("A product with the specified SKU already exists");

public record CategoryNotExistsError(string Category, int? ProductId)
    : Error("The specified category doesn't seem to exist");
    
public record InvalidProductPriceError() : Error("Price cannot be negative");

public record InvalidProductStockError() : Error("Stock cannot be negative");