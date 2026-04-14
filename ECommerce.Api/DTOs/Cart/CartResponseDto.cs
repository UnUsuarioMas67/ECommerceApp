namespace ECommerce.Api.DTOs.Cart;

public class CartResponseDto
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public ICollection<CartItemResponse> Items { get; set; }
    public decimal TotalPrice { get; set; }
    public int TotalProducts { get; set; }
}