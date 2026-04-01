namespace ECommerce.Api.Errors;

public record OrderNotExistsError(int OrderId) : Error($"ShopOrder with id {OrderId} not found");
public record OrderPaymentAlreadyExistsError(int OrderId) 
    : Error($"ShopOrder with id {OrderId} already has a payment attached");