using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Errors;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Application.Services.Payment;

public interface IOrderBuilderService
{
    Task<Result<ShopOrder>> BuildAsync(int cartId, int addressId, int? clientId = null);
}

public class OrderBuilderService(ECommerceContext context) : IOrderBuilderService
{
    public async Task<Result<ShopOrder>> BuildAsync(int cartId, int addressId, int? clientId = null)
    {
        var cartResult = await GetAndValidateCartAsync(cartId, clientId);
        if (!cartResult.IsSuccess)
            return cartResult.Error;
        var cart = cartResult.Value;

        var addressResult = await GetAndValidateAddressAsync(addressId, clientId);
        if (!addressResult.IsSuccess)
            return addressResult.Error;
        var address = addressResult.Value;

        var order = new ShopOrder
        {
            ClientId = cart.ClientId,
            AddressId = addressId,
            Address = address,
            OrderDate = DateTime.UtcNow,
            StatusId = OrderStatuses.Pending,
            Items = cart.Items.Select(item => new OrderLine
            {
                ProductId = item.ProductId,
                Product = item.Product,
                Quantity = item.Quantity,
                UnitPrice = item.Product.Price
            }).ToList()
        };

        foreach (var item in cart.Items)
            item.Product.Stock -= item.Quantity;

        context.ShopOrders.Add(order);
        context.Carts.Remove(cart);

        return order;
    }

    private async Task<Result<Cart>> GetAndValidateCartAsync(int cartId, int? clientId = null)
    {
        var query = context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .Where(c => c.Id == cartId);

        if (clientId.HasValue)
            query = query.Where(c => c.ClientId == clientId.Value);

        var cart = await query.FirstOrDefaultAsync();

        if (cart == null)
            return new CartNotFoundError(cartId);

        if (cart.Items.Count == 0)
            return new CartIsEmptyError(cartId);

        var productsWithoutStock = cart.Items
            .Where(i => i.Product.Stock < i.Quantity)
            .Select(i => new ProductsStockErrorItem(i.Product.Id, i.Quantity, i.Product.Stock))
            .ToList();
        if (productsWithoutStock.Count != 0)
            return new ProductsStockError(productsWithoutStock);

        return cart;
    }

    private async Task<Result<Address>> GetAndValidateAddressAsync(int addressId, int? clientId = null)
    {
        var query = context.Addresses
            .Where(a => a.Id == addressId);

        if (clientId.HasValue)
            query = query.Where(a => a.ClientId == clientId.Value);

        var address = await query.FirstOrDefaultAsync();

        if (address == null)
            return new AddressNotFoundError(addressId);

        return address;
    }
}
