using ECommerce.Api.DTOs.Error;

namespace ECommerce.Api.Errors;

public record DuplicateProductSkuError(string Sku)
    : Error("A product with the specified SKU already exists", "product_sku_exists")
{
    public override ErrorDto ToDto() => new()
    {
        ErrorType = ErrorType,
        Message = Message,
        Details = new Dictionary<string, object?>
        {
            ["sku"] = Sku,
        }
    };
}

public record CategoryNotExistsError(string Category)
    : Error("The specified category doesn't seem to exist", "category_not_found")
{
    public override ErrorDto ToDto() => new()
    {
        ErrorType = ErrorType,
        Message = Message,
        Details = new Dictionary<string, object?>
        {
            ["category"] = Category,
        }
    };
}

public record InvalidProductPriceError() : Error("Price cannot be negative", "invalid_price");

public record InvalidProductStockError() : Error("Stock cannot be negative", "invalid_stock");

public record ImageFileError(string Message) : Error(Message, "invalid_image"); 