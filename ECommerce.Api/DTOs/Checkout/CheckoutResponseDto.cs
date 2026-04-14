using ECommerce.Api.DTOs.Order;

namespace ECommerce.Api.DTOs.Checkout;

public class CheckoutResponseDto
{
    public string SessionId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public OrderResponseDto Order { get; set; }
}
