using ECommerce.Api.EF;
using ECommerce.Api.Entities;
using ECommerce.Api.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ECommerce.Api.Services.Checkout;

public class BackgroundOrderExpiryManager : BackgroundService
{
    private readonly OrderSettings _orderSettings;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundOrderExpiryManager> _logger;

    public BackgroundOrderExpiryManager(
        IServiceProvider serviceProvider,
        IOptions<OrderSettings> orderSettings,
        ILogger<BackgroundOrderExpiryManager> logger)
    {
        _orderSettings = orderSettings.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    private async Task ExpireOrdersAsync(ECommerceContext context)
    {
        var pendingOrders = await context.ShopOrders
            .Where(o => o.StatusId == OrderStatuses.Pending)
            .ToListAsync();

        var ordersToExpire = pendingOrders.Where(o =>
            DateTime.UtcNow - o.OrderDate >= TimeSpan.FromMinutes(_orderSettings.OrderExpireMinutes));

        foreach (var order in ordersToExpire)
        {
            order.StatusId = OrderStatuses.Expired;
            _logger.LogInformation("Order {orderId} has expired at {timestamp}", order.Id, DateTime.UtcNow);
        }

        await context.SaveChangesAsync();
    }

    private async Task DeleteExpiredOrdersAsync(ECommerceContext context)
    {
        var expiredOrders = await context.ShopOrders
            .Where(o => o.StatusId == OrderStatuses.Expired)
            .ToListAsync();

        var ordersToDelete = expiredOrders.Where(o =>
                DateTime.UtcNow - o.OrderDate >= TimeSpan.FromHours(_orderSettings.ExpiredOrderDeleteHours))
            .ToList();

        context.ShopOrders.RemoveRange(ordersToDelete);
        foreach (var order in ordersToDelete)
            _logger.LogInformation("Deleted expired order {orderId} at {timestamp}", order.Id, DateTime.UtcNow);

        await context.SaveChangesAsync();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(5));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            _logger.LogInformation("Running order expiry check at {timestamp}", DateTime.UtcNow);

            using var scope = _serviceProvider.CreateScope();
            await using var context = scope.ServiceProvider.GetRequiredService<ECommerceContext>();

            await ExpireOrdersAsync(context);
            await DeleteExpiredOrdersAsync(context);
        }
    }
}