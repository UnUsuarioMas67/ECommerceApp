using ECommerce.Api.Application.DTOs.Order;
using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Application.Services.Mapping;
using ECommerce.Api.EF;

using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Application.Services.DataAccess;

public interface IOrderService
{
    Task<OrderResponseDto?> GetByIdAsync(int orderId, int? clientId = null);
    Task<IEnumerable<OrderResponseDto>> GetManyAsync(PaginationQuery pagination);
    Task<IEnumerable<OrderResponseDto>> GetByClientAsync(int clientId, PaginationQuery pagination);
    Task<IEnumerable<OrderResponseDto>> GetByProductAsync(int productId, PaginationQuery pagination);
}

public class OrderService(ECommerceContext context, OrderMapper mapper) : IOrderService
{    public async Task<OrderResponseDto?> GetByIdAsync(int orderId, int? clientId = null)
    {
        var query = context.ShopOrders
            .AsNoTracking()
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.Id == orderId);

        if (clientId.HasValue)
            query = query.Where(o => o.ClientId == clientId.Value);

        var order = await query.FirstOrDefaultAsync();

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