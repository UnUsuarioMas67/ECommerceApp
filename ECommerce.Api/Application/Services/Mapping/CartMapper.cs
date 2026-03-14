using ECommerce.Api.Application.DTOs.Cart;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Application.Services.Mapping;

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

    public async Task<Cart> MapToEntityAsync(CartCreateDto dto)
    {
        var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == dto.ClientId);
        
        var cart = new Cart { ClientId = dto.ClientId, Client = client };
        cart.Items = await cartItemMapper.MapListToEntitiesAsync(cart, dto.Items.ToList());
        return cart;
    }

    public async Task ApplyUpdateAsync(Cart toUpdate, CartUpdateDto dto)
    {
        toUpdate.Items = await cartItemMapper.MapListToEntitiesAsync(toUpdate, dto.Items.ToList());
    }
}