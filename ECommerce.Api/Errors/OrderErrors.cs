using System.Linq;
using ECommerce.Api.DTOs.Error;

namespace ECommerce.Api.Errors;

public record CartNotFoundError(int? CartId)
    : Error("Could not find cart", "cart_not_found")
{
    public override ErrorDto ToDto() => new()
    {
        ErrorType = ErrorType,
        Message = Message,
        Details = new Dictionary<string, object?>
        {
            ["cartId"] = CartId
        }
    };
}

public record CartIsEmptyError(int? CartId)
    : Error("Cart cannot be empty", "cart_empty")
{
    public override ErrorDto ToDto() => new()
    {
        ErrorType = ErrorType,
        Message = Message,
        Details = new Dictionary<string, object?>
        {
            ["cartId"] = CartId
        }
    };
}

public record AddressNotFoundError(int AddressId)
    : Error("Could not find address", "address_not_found")
{
    public override ErrorDto ToDto() => new()
    {
        ErrorType = ErrorType,
        Message = Message,
        Details = new Dictionary<string, object?>
        {
            ["addressId"] = AddressId
        }
    };
}

public record ProductsStockError(ICollection<ProductsStockErrorItem> Products)
    : Error("One or more products have insufficient stock", "insufficient_stock")
{
    public override ErrorDto ToDto() => new()
    {
        ErrorType = ErrorType,
        Message = Message,
        Details = new Dictionary<string, object?>
        {
            ["products"] = Products.Select(p => new { p.ProductId, p.QuantityRequested, p.StockAvailable })
        }
    };
}
public record ProductsStockErrorItem(int ProductId, int QuantityRequested, int StockAvailable);

public record WebhookError() : Error("Webhook processing failed", "webhook_processing_failed");