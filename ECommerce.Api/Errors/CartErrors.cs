namespace ECommerce.Api.Errors;

public record ProductsNotExistError(int[] MissingProductIds)
    : Error("One or more of the specified products don't seem to exist", "products_not_found");

public record DuplicateItemsError(int[] DuplicateProductIds)
    : Error("The cart contains duplicate items", "duplicate_cart_items");

public record InvalidQuantityError(int[] InvalidProductIds)
    : Error("Item quantities must be greater than zero", "invalid_item_quantity");