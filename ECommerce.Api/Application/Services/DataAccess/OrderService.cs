using ECommerce.Api.Application.DTOs.Order;
using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Application.Services.Mapping;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Errors;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Application.Services.DataAccess;

public interface IOrderService
{
    Task<Result<ShopOrder>> CreateAsync(int cartId, int addressId);
    Task<OrderResponseDto?> GetByIdAsync(int orderId);
    Task<IEnumerable<OrderResponseDto>> GetManyAsync(PaginationQuery pagination);
    Task<IEnumerable<OrderResponseDto>> GetByClientAsync(int clientId, PaginationQuery pagination);
    Task<IEnumerable<OrderResponseDto>> GetByProductAsync(int productId, PaginationQuery pagination);
}

public class OrderService(ECommerceContext context, OrderMapper mapper) : IOrderService
{
    public async Task<Result<ShopOrder>> CreateAsync(int cartId, int addressId)
    {
        var cart = await context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.Id == cartId);

        if (cart == null)
            return new CartNotFoundError(cartId);

        if (!cart.Items.Any())
            return new CartIsEmptyError(cartId);

        var address = await context.Addresses.FindAsync(addressId);
        if (address == null)
            return new AddressNotFoundError(addressId);

        var order = new ShopOrder
        {
            ClientId = cart.ClientId,
            AddressId = addressId,
            OrderDate = DateTime.UtcNow,
            StatusId = 1,
            Items = cart.Items.Select(item => new OrderLine
            {
                ProductId = item.ProductId,
                Product = item.Product,
                Quantity = item.Quantity,
                UnitPrice = item.Product.Price
            }).ToList()
        };

        context.ShopOrders.Add(order);
        context.Carts.Remove(cart);
        
        await context.SaveChangesAsync();

        return order;
    }

    public async Task<OrderResponseDto?> GetByIdAsync(int orderId)
    {
        var order = await context.ShopOrders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        return order != null ? mapper.MapToDto(order) : null;
    }

    public async Task<IEnumerable<OrderResponseDto>> GetManyAsync(PaginationQuery pagination)
    {
        var orders = await context.ShopOrders
            .AsNoTracking()
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Skip(pagination.LimitOrDefault * (pagination.PageOrDefault - 1))
            .Take(pagination.LimitOrDefault)
            .ToListAsync();

        return orders.Select(mapper.MapToDto);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetByClientAsync(int clientId, PaginationQuery pagination)
    {
        var orders = await context.ShopOrders
            .AsNoTracking()
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.ClientId == clientId)
            .Skip(pagination.LimitOrDefault * (pagination.PageOrDefault - 1))
            .Take(pagination.LimitOrDefault)
            .ToListAsync();

        return orders.Select(mapper.MapToDto);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetByProductAsync(int productId, PaginationQuery pagination)
    {
        var orders = await context.ShopOrders
            .AsNoTracking()
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.Items.Any(i => i.ProductId == productId))
            .Skip(pagination.LimitOrDefault * (pagination.PageOrDefault - 1))
            .Take(pagination.LimitOrDefault)
            .ToListAsync();

        return orders.Select(mapper.MapToDto);
    }
}