namespace ECommerce.Dashboard.DTOs.Order;

public class OrderResponse
{
    public int Id { get; set; }
    public int? ClientId { get; set; }
    public int AddressId { get; set; }
    public DateTime OrderDate { get; set; }
    public ICollection<OrderLineResponse> Items { get; set; } = [];
    public string Status { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public int TotalProducts { get; set; }
}