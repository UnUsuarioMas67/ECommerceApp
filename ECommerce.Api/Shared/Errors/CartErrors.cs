namespace ECommerce.Api.Shared.Errors;

public record ProductsNotExistError(int[] MissingProductIds)
    : Error("One or more of the specified products don't seem to exist");

public record DuplicateItemsError(int[] DuplicateProductIds)
    : Error("The cart contains duplicate items");

public record InvalidQuantityError(int[] InvalidProductIds)
    : Error("Item quantities must be greater than zero");