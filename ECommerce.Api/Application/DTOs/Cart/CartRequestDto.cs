namespace ECommerce.Api.Application.DTOs.Cart;

public class CartRequestDto
{
    public ICollection<CartItemCreate> Items { get; set; }
}