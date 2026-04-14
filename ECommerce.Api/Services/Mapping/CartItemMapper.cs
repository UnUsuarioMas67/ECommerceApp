using ECommerce.Api.DTOs.Cart;
using ECommerce.Api.EF;
using ECommerce.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Services.Mapping;

public class CartItemMapper(ProductMapper productMapper, ECommerceContext context)
{
    public CartItemResponse MapToDto(CartItem item)
    {
        if (item.Product == null)
            throw new InvalidOperationException($"{nameof(item.Product)} should not be null");

        return new CartItemResponse()
        {
            Product = productMapper.MapToDto(item.Product),
            Quantity = item.Quantity,
        };
    }

    public async Task<List<CartItem>> MapListToEntitiesAsync(Cart cart, List<CartItemCreate> items)
    {
        var groupedItems = items.GroupBy(i => i.ProductId)
            .Select(g =>
            {
                var first = g.First();
                first.Quantity = g.Sum(i => i.Quantity);
                return first;
            })
            .ToList();

        var itemProductIds = groupedItems.Select(i => i.ProductId).Distinct();

        var products = await context.Products
            .Include(p => p.Category)
            .Where(p => itemProductIds.Contains(p.Id))
            .ToListAsync();

        return groupedItems.Select(i => new CartItem
        {
            Cart = cart,
            CartId = cart.Id,
            Quantity = i.Quantity,
            ProductId = i.ProductId,
            Product = products.FirstOrDefault(p => p.Id == i.ProductId)
        }).ToList();
    }
}