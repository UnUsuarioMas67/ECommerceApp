using ECommerce.Api.DTOs.Error;

namespace ECommerce.Api.Errors;

public record OrderNotExistsError(int OrderId) : Error($"ShopOrder with id {OrderId} not found", "order_not_found")
{
    public new ErrorDto ToDto() => new()
    {
        ErrorType = ErrorType,
        Message = Message,
        Details = new Dictionary<string, object?>
        {
            ["orderId"] = OrderId
        }
    };
}
public record OrderPaymentAlreadyExistsError(int OrderId) 
    : Error($"ShopOrder with id {OrderId} already has a payment attached", "order_already_paid")
{
    public new ErrorDto ToDto() => new()
    {
        ErrorType = ErrorType,
        Message = Message,
        Details = new Dictionary<string, object?>
        {
            ["orderId"] = OrderId
        }
    };
}