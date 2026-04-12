using ECommerce.Api.Entities;

namespace ECommerce.Api.Application.DTOs.Checkout;

public class PaymentResultDto
{
    public int PaymentId { get; set; }
    public int OrderId { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
