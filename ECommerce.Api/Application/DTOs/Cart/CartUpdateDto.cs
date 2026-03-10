namespace ECommerce.Api.Application.DTOs.Cart;

public class CartUpdateDto
{
    public ICollection<CartItemEntry> Items { get; set; }
}