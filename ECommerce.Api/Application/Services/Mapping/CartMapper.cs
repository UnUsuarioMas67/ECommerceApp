using ECommerce.Api.Application.DTOs.Cart;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Application.Services.Mapping;

public interface ICartMapper : IEntityDtoAsyncMapper<Cart, CartResponseDto, CartCreateDto, CartUpdateDto>;

public class CartMapper(IProductMapper productMapper) : ICartMapper
{
    public CartResponseDto ToDto(Cart cart)
    {
        var dto = new CartResponseDto
        {
            Id = cart.Id,
            ClientId = cart.ClientId,
            Items = cart.Items.Select(ci => new CartItemDto
            {
                Quantity = ci.Quantity,
                Product = productMapper.ToDto(ci.Product
                                              ?? throw new InvalidOperationException("Product must be included")),
            }).ToList()
        };

        dto.TotalPrice = dto.Items.Sum(i => i.Product.Price * i.Quantity);
        dto.TotalProducts = dto.Items.Count;

        return dto;
    }

    public async Task<Cart> ToEntityAsync(CartCreateDto dto, ECommerceContext context)
    {
        var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == dto.ClientId);
        
        var cart = new Cart { ClientId = dto.ClientId, Client = client };
        cart.Items = await GetCartItems(cart, dto.Items, context);
        return cart;
    }

    public async Task ApplyUpdateToEntityAsync(Cart toUpdate, CartUpdateDto dto, ECommerceContext context)
    {
        toUpdate.Items = await GetCartItems(toUpdate, dto.Items, context);
    }

    private async Task<ICollection<CartItem>> GetCartItems(Cart cart, IEnumerable<CartItemEntry> items, ECommerceContext context)
    {
        var cartItemEntries = items.ToList();
        var itemProductIds = cartItemEntries.Select(i => i.ProductId).Distinct();
        
        var products = await context.Products
            .Include(p => p.Category)
            .Where(p => itemProductIds.Contains(p.Id))
            .ToListAsync();
        
        return cartItemEntries.Select(i => new CartItem
        {
            Cart = cart,
            Quantity = i.Quantity,
            ProductId = i.ProductId,
            Product = products.FirstOrDefault(p => p.Id == i.ProductId)
        }).ToList();
    }
}