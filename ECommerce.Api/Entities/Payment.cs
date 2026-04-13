namespace ECommerce.Api.Entities;

public class Payment
{
    public int Id { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "usd";

    public int OrderId { get; set; }
    public ShopOrder Order { get; set; }

    public DateTime CreatedAt { get; set; }
}