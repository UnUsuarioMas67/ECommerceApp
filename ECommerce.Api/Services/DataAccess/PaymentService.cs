using ECommerce.Api.Application.DTOs.Checkout;
using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.EF;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Services.DataAccess;

public interface IPaymentService
{
    Task<PaymentResultDto?> GetByIdAsync(int paymentId);
    Task<IEnumerable<PaymentResultDto>> GetManyAsync(PaginationQuery pagination);
    Task<PaymentResultDto?> GetByShopOrderAsync(int orderId);
}

public class PaymentService(ECommerceContext context) : IPaymentService
{
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

    private static PaymentResultDto MapToDto(Entities.Payment payment)
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
