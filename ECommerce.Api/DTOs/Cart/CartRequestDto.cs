namespace ECommerce.Api.DTOs.Cart;

public class CartRequestDto
{
    public ICollection<CartItemCreate> Items { get; set; }
}