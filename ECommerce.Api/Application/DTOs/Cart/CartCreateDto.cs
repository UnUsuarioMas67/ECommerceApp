namespace ECommerce.Api.Application.DTOs.Cart;

public class CartCreateDto
{
    public int ClientId { get; set; }
    public ICollection<CartItemCreate> Items { get; set; }
}