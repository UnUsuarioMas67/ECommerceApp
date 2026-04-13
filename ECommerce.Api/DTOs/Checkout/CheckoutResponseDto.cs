using ECommerce.Api.Application.DTOs.Order;

namespace ECommerce.Api.Application.DTOs.Checkout;

public class CheckoutResponseDto
{
    public string SessionId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public OrderResponseDto Order { get; set; }
}
