namespace ECommerce.Api.Application.DTOs.Cart;

public class CartUpdateDto
{
    public ICollection<CartItemCreate> Items { get; set; }
}