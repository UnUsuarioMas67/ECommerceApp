namespace ECommerce.Api.Application.DTOs.Checkout;

public class CheckoutRequestDto
{
    public int CartId { get; set; }
    public int AddressId { get; set; }
    public string SuccessUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
}
