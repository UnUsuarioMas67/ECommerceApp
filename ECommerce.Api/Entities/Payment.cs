namespace ECommerce.Api.Entities;

public enum PaymentStatus
{
    Pending = 1,
    Succeeded = 2,
    Failed = 3
}

public class Payment
{
    public int Id { get; set; }
    public string StripePaymentIntentId { get; set; } = string.Empty;
    public string StripeSessionId { get; set; } = string.Empty;
    
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "usd";
    
    public int StatusId { get; set; } = (int)PaymentStatus.Pending;
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    
    public int OrderId { get; set; }
    public ShopOrder Order { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
