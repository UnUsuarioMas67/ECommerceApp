using ECommerce.Api.DTOs.Address;
using ECommerce.Api.DTOs.User;

namespace ECommerce.Api.DTOs.Order;

public class OrderResponseDto
{
    public int Id { get; set; }
    public int? ClientId { get; set; }
    public string? ClientEmail { get; set; }
    public string? ClientName { get; set; }
    public AddressResponseDto Address { get; set; }
    public DateTime OrderDate { get; set; }
    public ICollection<OrderLineResponseDto> Items { get; set; }
    public string Status { get; set; }
    public decimal TotalPrice { get; set; }
    public int TotalProducts { get; set; }
}
