using ECommerce.Api.EF;
using ECommerce.Api.Entities;
using ECommerce.Api.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe.Checkout;

namespace ECommerce.Api.Services.Checkout;

public class BackgroundOrderExpiryManager : BackgroundService
{
    private readonly int _checkExpiryMinutes;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundOrderExpiryManager> _logger;

    public BackgroundOrderExpiryManager(
        IServiceProvider serviceProvider,
        IOptions<OrderExpirySettings> orderSettings,
        ILogger<BackgroundOrderExpiryManager> logger)
    {
        _checkExpiryMinutes = orderSettings.Value.CheckExpiryMinutes;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    private async Task ExpireOrdersAsync(ECommerceContext context, SessionService sessionService)
    {
        var ordersToExpire = await context.ShopOrders
            .Where(o => o.StatusId == OrderStatuses.Pending)
            .Where(o => o.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync();
        
        foreach (var order in ordersToExpire)
        {
            order.StatusId = OrderStatuses.Expired;
            await sessionService.ExpireAsync(order.StripeSessionId);
            _logger.LogInformation("Order {orderId} has expired at {timestamp}", order.Id, DateTime.UtcNow);
            _logger.LogInformation("Expired checkout session {sessionId}", order.StripeSessionId);
        }

        await context.SaveChangesAsync();
    }

    private async Task DeleteExpiredOrdersAsync(ECommerceContext context)
    {
        var ordersToDelete = await context.ShopOrders
            .Where(o => o.StatusId == OrderStatuses.Expired)
            .Where(o => o.DeleteIfExpiredAt <= DateTime.UtcNow)
            .ToListAsync();
        
        context.ShopOrders.RemoveRange(ordersToDelete);
        foreach (var order in ordersToDelete)
            _logger.LogInformation("Deleted expired order {orderId} at {timestamp}", order.Id, DateTime.UtcNow);
        
        await context.SaveChangesAsync();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(_checkExpiryMinutes));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            _logger.LogInformation("Running order expiry check at {timestamp}", DateTime.UtcNow);

            using var scope = _serviceProvider.CreateScope();
            await using var context = scope.ServiceProvider.GetRequiredService<ECommerceContext>();
            var sessionService = scope.ServiceProvider.GetRequiredService<SessionService>();

            await ExpireOrdersAsync(context, sessionService);
            await DeleteExpiredOrdersAsync(context);
        }
    }
}