using ECommerce.Api.DTOs.Order;
using ECommerce.Api.DTOs.Shared;
using ECommerce.Api.EF;
using ECommerce.Api.Entities;
using ECommerce.Api.Services.Mapping;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;

namespace ECommerce.Api.Services.DataAccess;

public interface IOrderService
{
    Task<OrderResponseDto?> GetByIdAsync(int orderId, int? clientId = null);
    Task<IEnumerable<OrderResponseDto>> GetManyAsync(PaginationQuery pagination);
    Task<IEnumerable<OrderResponseDto>> GetByClientAsync(int clientId, PaginationQuery pagination);
    Task<IEnumerable<OrderResponseDto>> GetByProductAsync(int productId, PaginationQuery pagination);
    Task ExpireOrdersAsync();
    Task DeleteExpiredOrdersAsync();
}

public class OrderService(
    ECommerceContext context,
    OrderMapper mapper,
    ILogger<OrderService> logger,
    SessionService sessionService) : IOrderService
{
    public async Task<OrderResponseDto?> GetByIdAsync(int orderId, int? clientId = null)
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
            .OrderByDescending(o => o.OrderDate)
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
            .OrderByDescending(o => o.OrderDate)
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
            .OrderByDescending(o => o.OrderDate)
            .Skip(pagination.LimitOrDefault * (pagination.PageOrDefault - 1))
            .Take(pagination.LimitOrDefault)
            .ToListAsync();

        return orders.Select(mapper.MapToDto);
    }

    public async Task ExpireOrdersAsync()
    {
        var ordersToExpire = await context.ShopOrders
            .Where(o => o.StatusId == OrderStatuses.Pending)
            .Where(o => o.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync();

        foreach (var order in ordersToExpire)
        {
            order.StatusId = OrderStatuses.Expired;
            logger.LogInformation("Order {orderId} has expired at {timestamp}", order.Id, DateTime.UtcNow);

            var session = await sessionService.GetAsync(order.StripeSessionId);
            if (session.Status == "open") 
            {
                await sessionService.ExpireAsync(order.StripeSessionId);
                logger.LogInformation("Expired checkout session {sessionId}", order.StripeSessionId);
            }
        }

        await context.SaveChangesAsync();
    }

    public async Task DeleteExpiredOrdersAsync()
    {
        var ordersToDelete = await context.ShopOrders
            .Where(o => o.StatusId == OrderStatuses.Expired)
            .Where(o => o.DeleteIfExpiredAt <= DateTime.UtcNow)
            .ToListAsync();

        context.ShopOrders.RemoveRange(ordersToDelete);
        foreach (var order in ordersToDelete)
            logger.LogInformation("Deleted expired order {orderId} at {timestamp}", order.Id, DateTime.UtcNow);

        await context.SaveChangesAsync();
    }
}