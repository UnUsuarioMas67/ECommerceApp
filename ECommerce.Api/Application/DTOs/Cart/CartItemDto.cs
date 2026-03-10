using ECommerce.Api.Application.DTOs.Product;

namespace ECommerce.Api.Application.DTOs.Cart;

public class CartItemDto
{
    public ProductResponseDto Product { get; set; }
    public int Quantity { get; set; }
}

public class CartItemEntry
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}