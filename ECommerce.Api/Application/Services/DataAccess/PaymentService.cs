using ECommerce.Api.Application.DTOs.Checkout;
using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Errors;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Application.Services.DataAccess;

public interface IPaymentService
{
    Task<Result<Domain.Entities.Payment>> CreateAsync(int orderId);
    Task<PaymentResultDto?> GetByIdAsync(int paymentId);
    Task<IEnumerable<PaymentResultDto>> GetManyAsync(PaginationQuery pagination);
    Task<PaymentResultDto?> GetByShopOrderAsync(int orderId);
}

public class PaymentService(ECommerceContext context) : IPaymentService
{
    public async Task<Result<Domain.Entities.Payment>> CreateAsync(int orderId)
    {
        var order = await context.ShopOrders
            .Include(o => o.Items)
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == orderId);
        
        if (order == null)
            return new OrderNotExistsError(orderId);
        if (order.Payment != null)
            return new OrderPaymentAlreadyExistsError(orderId);

        var totalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);

        var payment = new Domain.Entities.Payment
        {
            OrderId = orderId,
            Order = order,
            Amount = totalAmount,
            Currency = "usd",
            StatusId = (int)PaymentStatus.Pending,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        context.Payments.Add(payment);
        await context.SaveChangesAsync();

        return payment;
    }

    public async Task<PaymentResultDto?> GetByIdAsync(int paymentId)
    {
        var payment = await context.Payments
            .FirstOrDefaultAsync(p => p.Id == paymentId);

        return payment != null ? MapToDto(payment) : null;
    }

    public async Task<IEnumerable<PaymentResultDto>> GetManyAsync(PaginationQuery pagination)
    {
        var payments = await context.Payments
            .Skip(pagination.LimitOrDefault * (pagination.PageOrDefault - 1))
            .Take(pagination.LimitOrDefault)
            .ToListAsync();

        return payments.Select(MapToDto);
    }

    public async Task<PaymentResultDto?> GetByShopOrderAsync(int orderId)
    {
        var payment = await context.Payments
            .FirstOrDefaultAsync(p => p.OrderId == orderId);

        return payment != null ? MapToDto(payment) : null;
    }

    private static PaymentResultDto MapToDto(Domain.Entities.Payment payment)
    {
        return new PaymentResultDto
        {
            PaymentId = payment.Id,
            OrderId = payment.OrderId,
            SessionId = payment.StripeSessionId,
            Status = payment.Status,
            Amount = payment.Amount,
            Currency = payment.Currency,
            CreatedAt = payment.CreatedAt
        };
    }
}
