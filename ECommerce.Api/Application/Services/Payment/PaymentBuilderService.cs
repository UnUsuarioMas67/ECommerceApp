using ECommerce.Api.EF;
using ECommerce.Api.Entities;
using ECommerce.Api.Errors;

using ECommerce.Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Application.Services.Payment;

public interface IPaymentBuilderService
{
    Task<Result<Entities.Payment>> BuildAsync(ShopOrder order);
}

public class PaymentBuilderService(ECommerceContext context) : IPaymentBuilderService
{
    public async Task<Result<Entities.Payment>> BuildAsync(ShopOrder order)
    {
        // var order = await context.ShopOrders
        //     .Include(o => o.Items)
        //     .Include(o => o.Payment)
        //     .FirstOrDefaultAsync(o => o.Id == orderId);
        //
        // if (order == null)
        //     return new OrderNotExistsError(orderId);
        if (order.Payment != null)
            return new OrderPaymentAlreadyExistsError(order.Id);

        var totalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);

        var payment = new Entities.Payment
        {
            Order = order,
            Amount = totalAmount,
            Currency = "usd",
            StatusId = (int)PaymentStatus.Pending,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        context.Payments.Add(payment);

        return payment;
    }
}
