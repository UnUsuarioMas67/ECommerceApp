using ECommerce.Api.DTOs.Error;

namespace ECommerce.Api.Errors;

public record ProductsNotExistError(int[] MissingProductIds)
    : Error("One or more of the specified products don't seem to exist", "products_not_found")
{
    public new ErrorDto ToDto() => new()
    {
        ErrorType = ErrorType,
        Message = Message,
        Details = new Dictionary<string, object?>
        {
            ["missingProductIds"] = MissingProductIds
        }
    };
}

public record DuplicateItemsError(int[] DuplicateProductIds)
    : Error("The cart contains duplicate items", "duplicate_cart_items")
{
    public new ErrorDto ToDto() => new()
    {
        ErrorType = ErrorType,
        Message = Message,
        Details = new Dictionary<string, object?>
        {
            ["duplicateProductIds"] = DuplicateProductIds
        }
    };
}

public record InvalidQuantityError(int[] InvalidProductIds)
    : Error("Item quantities must be greater than zero", "invalid_item_quantity")
{
    public new ErrorDto ToDto() => new()
    {
        ErrorType = ErrorType,
        Message = Message,
        Details = new Dictionary<string, object?>
        {
            ["invalidProductIds"] = InvalidProductIds
        }
    };
}