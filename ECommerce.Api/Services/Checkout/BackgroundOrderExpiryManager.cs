using ECommerce.Api.EF;
using ECommerce.Api.Entities;
using ECommerce.Api.Services.DataAccess;
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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(_checkExpiryMinutes));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            _logger.LogInformation("Running order expiry check at {timestamp}", DateTime.UtcNow);

            using var scope = _serviceProvider.CreateScope();
            var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

            await orderService.ExpireOrdersAsync();
            await orderService.DeleteExpiredOrdersAsync();
        }
    }
}