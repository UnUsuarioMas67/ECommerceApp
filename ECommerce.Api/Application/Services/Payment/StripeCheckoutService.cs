using ECommerce.Api.Application.DTOs.Checkout;
using ECommerce.Api.Application.Services.DataAccess;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Infrastructure.EF;
using ECommerce.Api.Settings;
using ECommerce.Api.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;

namespace ECommerce.Api.Application.Services.Payment;

public interface IStripeCheckoutService
{
    Task<Result<CheckoutResponseDto>> CreateCheckoutSessionAsync(CheckoutRequestDto request);
    Task<PaymentResultDto?> ProcessWebhookAsync(string payload, string signature);
    Task<PaymentResultDto?> GetPaymentBySessionIdAsync(string sessionId);
}

public class StripeCheckoutService : IStripeCheckoutService
{
    private readonly ECommerceContext _context;
    private readonly StripeSettings _stripeSettings;
    private readonly ILogger<StripeCheckoutService> _logger;
    private readonly IOrderService _orderService;
    private readonly IPaymentService _paymentService;

    public StripeCheckoutService(
        ECommerceContext context,
        IOptions<StripeSettings> stripeSettings,
        ILogger<StripeCheckoutService> logger, 
        IOrderService orderService, 
        IPaymentService paymentService)
    {
        _context = context;
        _stripeSettings = stripeSettings.Value;
        _logger = logger;
        _orderService = orderService;
        _paymentService = paymentService;
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
    }

    public async Task<Result<CheckoutResponseDto>> CreateCheckoutSessionAsync(CheckoutRequestDto request)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        
        var orderResult = await _orderService.CreateAsync(request.CartId, request.AddressId);
        if (!orderResult.IsSuccess)
        {
            await transaction.RollbackAsync();
            return orderResult.Error;
        }
        var order = orderResult.Value ?? throw new InvalidOperationException();
        
        var paymentResult = await _paymentService.CreateAsync(order.Id);
        if (!paymentResult.IsSuccess)
        {
            await transaction.RollbackAsync();
            return paymentResult.Error;
        }
        var payment = paymentResult.Value ?? throw new InvalidOperationException();
        
        var session = await CreateStripeSessionAsync(request, order, payment);

        payment.StripeSessionId = session.Id;
        payment.StripePaymentIntentId = session.PaymentIntentId ?? string.Empty;
        
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created Stripe session {SessionId} for order {OrderId}", session.Id, order.Id);
        
        await transaction.CommitAsync();

        return new CheckoutResponseDto
        {
            SessionId = session.Id,
            Url = session.Url,
            Amount = payment.Amount,
            Currency = "usd"
        };
    }

    private List<SessionLineItemOptions> BuildLineItems(ShopOrder order)
    {
        return order.Items.Select(item => new SessionLineItemOptions
        {
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = "usd",
                UnitAmount = (long)(item.Product.Price * 100),
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = item.Product.Name,
                    Description = item.Product.Description?.Length > 500
                        ? item.Product.Description[..500]
                        : item.Product.Description
                }
            },
            Quantity = item.Quantity
        }).ToList();
    }

    private async Task<Session> CreateStripeSessionAsync(
        CheckoutRequestDto request,
        ShopOrder order,
        Domain.Entities.Payment payment)
    {
        var sessionOptions = new SessionCreateOptions
        {
            PaymentMethodTypes = ["card"],
            LineItems = BuildLineItems(order),
            Mode = "payment",
            Metadata = new Dictionary<string, string>
            {
                { "orderId", order.Id.ToString() },
                { "paymentId", payment.Id.ToString() },
            }
        };

        if (!string.IsNullOrWhiteSpace(request.SuccessUrl))
            sessionOptions.SuccessUrl = request.SuccessUrl;
        if (!string.IsNullOrWhiteSpace(request.CancelUrl))
            sessionOptions.CancelUrl = request.CancelUrl;

        return await new SessionService().CreateAsync(sessionOptions);
    }

    public async Task<PaymentResultDto?> ProcessWebhookAsync(string payload, string signature)
    {
        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(payload, signature, _stripeSettings.WebhookSecret);
        }
        catch (StripeException ex)
        {
            _logger.LogError("Webhook signature verification failed: {Error}", ex.Message);
            return null;
        }

        if (stripeEvent.Type == "checkout.session.completed")
        {
            var session = stripeEvent.Data.Object as Session;
            if (session == null) return null;

            return await HandleCheckoutCompleted(session);
        }

        if (stripeEvent.Type == "payment_intent.succeeded")
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent == null) return null;

            return await HandlePaymentIntentSucceeded(paymentIntent);
        }

        return null;
    }

    private async Task<PaymentResultDto?> HandleCheckoutCompleted(Session session)
    {
        var payment = await _context.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.StripeSessionId == session.Id);

        if (payment == null)
        {
            _logger.LogWarning("Payment not found for session {SessionId}", session.Id);
            return null;
        }

        payment.StatusId = (int)PaymentStatus.Succeeded;
        payment.Status = PaymentStatus.Succeeded;
        payment.StripePaymentIntentId = session.PaymentIntentId ?? payment.StripePaymentIntentId;
        payment.UpdatedAt = DateTime.UtcNow;

        payment.Order.StatusId = 2;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Payment {PaymentId} completed for order {OrderId}", payment.Id, payment.OrderId);

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

    private async Task<PaymentResultDto?> HandlePaymentIntentSucceeded(PaymentIntent paymentIntent)
    {
        var payment = await _context.Payments
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.StripePaymentIntentId == paymentIntent.Id);

        if (payment == null)
        {
            _logger.LogWarning("Payment not found for payment intent {PaymentIntentId}", paymentIntent.Id);
            return null;
        }

        if (payment.Status != PaymentStatus.Succeeded)
        {
            payment.StatusId = (int)PaymentStatus.Succeeded;
            payment.Status = PaymentStatus.Succeeded;
            payment.UpdatedAt = DateTime.UtcNow;

            payment.Order.StatusId = 2;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Payment {PaymentId} succeeded for order {OrderId}", payment.Id, payment.OrderId);
        }

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
    
    public async Task<PaymentResultDto?> GetPaymentBySessionIdAsync(string sessionId)
    {
        var payment = await _context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.StripeSessionId == sessionId);

        if (payment == null) return null;

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
