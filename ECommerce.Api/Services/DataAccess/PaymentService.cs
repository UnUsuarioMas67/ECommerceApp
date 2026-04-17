using ECommerce.Api.DTOs.Checkout;
using ECommerce.Api.DTOs.Shared;
using ECommerce.Api.EF;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Services.DataAccess;

public interface IPaymentService
{
    Task<PaymentResultDto?> GetByIdAsync(int paymentId);
    Task<IEnumerable<PaymentResultDto>> GetManyAsync(PaginationQuery pagination);
    Task<PaymentResultDto?> GetByShopOrderAsync(int orderId);
    Task<PaymentResultDto?> GetPaymentBySessionIdAsync(string sessionId, int? clientId);
}

public class PaymentService(ECommerceContext context) : IPaymentService
{
    public async Task<PaymentResultDto?> GetByIdAsync(int paymentId)
    {
        var payment = await context.Payments.FindAsync(paymentId);

        return payment != null ? MapToDto(payment) : null;
    }

    public async Task<IEnumerable<PaymentResultDto>> GetManyAsync(PaginationQuery pagination)
    {
        var payments = await context.Payments
            .AsNoTracking()
            .Skip(pagination.LimitOrDefault * (pagination.PageOrDefault - 1))
            .Take(pagination.LimitOrDefault)
            .ToListAsync();

        return payments.Select(MapToDto);
    }

    public async Task<PaymentResultDto?> GetByShopOrderAsync(int orderId)
    {
        var payment = await context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.OrderId == orderId);

        return payment != null ? MapToDto(payment) : null;
    }

    private static PaymentResultDto MapToDto(Entities.Payment payment)
    {
        return new PaymentResultDto
        {
            PaymentId = payment.Id,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            Currency = payment.Currency,
            CreatedAt = payment.CreatedAt
        };
    }

    public async Task<PaymentResultDto?> GetPaymentBySessionIdAsync(string sessionId, int? clientId = null)
    {
        var query = context.Payments
            .AsNoTracking();

        if (clientId.HasValue)
            query = query.Where(p => p.Id == clientId.Value);

        var payment = await query
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.Order.StripeSessionId == sessionId);

        if (payment == null) return null;

        return new PaymentResultDto
        {
            PaymentId = payment.Id,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            Currency = payment.Currency,
            CreatedAt = payment.CreatedAt
        };
    }
}