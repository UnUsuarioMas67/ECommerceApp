namespace ECommerce.Api.Application.DTOs.Order;

public class OrderResponseDto
{
    public int Id { get; set; }
    public int? ClientId { get; set; }
    public int AddressId { get; set; }
    public DateTime OrderDate { get; set; }
    public ICollection<OrderLineResponseDto> Items { get; set; }
    public string Status { get; set; }
    public decimal TotalPrice { get; set; }
    public int TotalProducts { get; set; }
}
