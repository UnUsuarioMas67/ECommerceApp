using ECommerce.Api.Application.DTOs.Product;

namespace ECommerce.Api.Application.DTOs.Cart;

public class CartItemResponse
{
    public ProductResponseDto Product { get; set; }
    public int Quantity { get; set; }
}

public class CartItemCreate
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}