using ECommerce.Api.Application.DTOs.Cart;
using ECommerce.Api.EF;
using ECommerce.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Services.Mapping;

public class CartMapper(CartItemMapper cartItemMapper, ECommerceContext context)
{
    public CartResponseDto MapToDto(Cart cart)
    {
        var dto = new CartResponseDto
        {
            Id = cart.Id,
            ClientId = cart.ClientId,
            Items = cart.Items
                .Select(cartItemMapper.MapToDto)
                .ToList()
        };

        dto.TotalPrice = dto.Items.Sum(i => i.Product.Price * i.Quantity);
        dto.TotalProducts = dto.Items.Count;

        return dto;
    }

    public async Task<Cart> MapToEntityAsync(CartRequestDto dto, int clientId)
    {
        var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
        
        var cart = new Cart { ClientId = clientId, Client = client };
        cart.Items = await cartItemMapper.MapListToEntitiesAsync(cart, dto.Items.ToList());
        return cart;
    }

    public async Task ApplyUpdateAsync(Cart toUpdate, CartRequestDto dto)
    {
        toUpdate.Items = await cartItemMapper.MapListToEntitiesAsync(toUpdate, dto.Items.ToList());
    }
}