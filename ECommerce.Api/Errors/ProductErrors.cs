using ECommerce.Api.DTOs.Error;

namespace ECommerce.Api.Errors;

public record DuplicateProductSkuError(string Sku, int? ProductId)
    : Error("A product with the specified SKU already exists", "product_sku_exists")
{
    public new ErrorDto ToDto() => new()
    {
        ErrorType = ErrorType,
        Message = Message,
        Details = new Dictionary<string, object?>
        {
            ["sku"] = Sku,
            ["productId"] = ProductId
        }
    };
}

public record CategoryNotExistsError(string Category, int? ProductId)
    : Error("The specified category doesn't seem to exist", "category_not_found")
{
    public new ErrorDto ToDto() => new()
    {
        ErrorType = ErrorType,
        Message = Message,
        Details = new Dictionary<string, object?>
        {
            ["category"] = Category,
            ["productId"] = ProductId
        }
    };
}

public record InvalidProductPriceError() : Error("Price cannot be negative", "invalid_price");

public record InvalidProductStockError() : Error("Stock cannot be negative", "invalid_stock");