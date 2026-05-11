using ECommerce.Api.DTOs.Checkout;
using ECommerce.Api.EF;
using ECommerce.Api.Entities;
using ECommerce.Api.Errors;
using ECommerce.Api.Services.Mapping;
using ECommerce.Api.Settings;
using ECommerce.Api.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Address = ECommerce.Api.Entities.Address;

namespace ECommerce.Api.Services.Checkout;

public interface IStripeCheckoutService
{
    Task<Result<CheckoutResponseDto>> CreateCheckoutSessionAsync(CheckoutRequestDto request, int clientId);
    Task<Result> ProcessWebhookAsync(string payload, string signature);
}

public class StripeCheckoutService : IStripeCheckoutService
{
    private readonly ECommerceContext _context;
    private readonly SessionService _sessionService;
    private readonly OrderMapper _orderMapper;
    private readonly StripeSettings _stripeSettings;
    private readonly OrderExpirySettings _orderExpirySettings;
    private readonly ILogger<StripeCheckoutService> _logger;

    public StripeCheckoutService(
        ECommerceContext context,
        SessionService sessionService,
        IOptions<StripeSettings> stripeSettings,
        OrderMapper orderMapper,
        ILogger<StripeCheckoutService> logger,
        IOptions<OrderExpirySettings> orderSettings)
    {
        _context = context;
        _sessionService = sessionService;
        _orderMapper = orderMapper;
        _stripeSettings = stripeSettings.Value;
        _logger = logger;
        _orderExpirySettings = orderSettings.Value;
    }

    #region Checkout Session

    public async Task<Result<CheckoutResponseDto>> CreateCheckoutSessionAsync(CheckoutRequestDto request, int clientId)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        var client = await _context.Clients
            .Include(cl => cl.Addresses)
            .ThenInclude(a => a.Country)
            .Include(cl => cl.Carts)
            .ThenInclude(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(cl => cl.Id == clientId);

        if (client == null)
            return new ClientNotExistsError(clientId, null);

        var cart = client.Carts.FirstOrDefault(c => c.Id == request.CartId);
        if (cart == null)
            return new CartNotFoundError(request.CartId);
        if (cart.Items.Count == 0)
            return new CartIsEmptyError(request.CartId);
        
        var address = client.Addresses.FirstOrDefault(a => a.Id == request.AddressId);
        if (address == null)
            return new AddressNotFoundError(request.AddressId);

        var orderResult = CreateOrder(cart, address, client);
        if (!orderResult.IsSuccess)
        {
            await transaction.RollbackAsync();
            return orderResult.Error;
        }

        var order = orderResult.Value;

        var insufficientStockItems = await VerifyOrderProductsStockAsync(order);
        if (insufficientStockItems.Any())
        {
            await transaction.RollbackAsync();
            return new ProductsStockError(insufficientStockItems.ToList());
        }

        await _context.ShopOrders.AddAsync(order);
        _context.Carts.Remove(cart);
        await _context.SaveChangesAsync();

        var session = await CreateStripeSessionAsync(request, order);

        order.StripeSessionId = session.Id;
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        _logger.LogInformation("Created Stripe session {SessionId} for order {OrderId}", session.Id, order.Id);

        var paidAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);

        return new CheckoutResponseDto
        {
            SessionId = session.Id,
            Url = session.Url,
            Amount = paidAmount,
            Currency = "usd",
            ExpiresAt = order.ExpiresAt,
            Order = _orderMapper.MapToDto(order)
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
                    Description = item.Product.Description.Length > 500
                        ? item.Product.Description[..500]
                        : item.Product.Description
                }
            },
            Quantity = item.Quantity
        }).ToList();
    }

    private async Task<Session> CreateStripeSessionAsync(CheckoutRequestDto request, ShopOrder order)
    {
        var metadata = new Dictionary<string, string>
        {
            { "orderId", order.Id.ToString() },
        };

        var sessionOptions = new SessionCreateOptions
        {
            PaymentMethodTypes = ["card"],
            LineItems = BuildLineItems(order),
            Mode = "payment",
            Metadata = metadata,
            PaymentIntentData = new SessionPaymentIntentDataOptions
            {
                Metadata = metadata
            },
        };

        if (!string.IsNullOrWhiteSpace(request.SuccessUrl))
            sessionOptions.SuccessUrl = request.SuccessUrl;
        if (!string.IsNullOrWhiteSpace(request.CancelUrl))
            sessionOptions.CancelUrl = request.CancelUrl;

        return await _sessionService.CreateAsync(sessionOptions);
    }

    #region Create Order

    private Result<ShopOrder> CreateOrder(Cart cart, Address address, Client client)
    {
        var orderDate = DateTime.UtcNow;
        var expiresAt = orderDate.AddMinutes(_orderExpirySettings.ExpireMinutes);
        var deleteIfExpiredAt = expiresAt.AddHours(_orderExpirySettings.DeleteExpiredAfterHours);

        var order = new ShopOrder
        {
            ClientId = cart.ClientId,
            Client = client,
            
            AddressId = address.Id,
            Address = address,

            OrderDate = orderDate,
            ExpiresAt = expiresAt,
            DeleteIfExpiredAt = deleteIfExpiredAt,

            Items = cart.Items.Select(item => new OrderLine
            {
                ProductId = item.ProductId,
                Product = item.Product,
                Quantity = item.Quantity,
                UnitPrice = item.Product.Price
            }).ToList(),
            
            StatusId = OrderStatuses.Pending,
        };

        return order;
    }

    private async Task<List<ProductsStockErrorItem>> VerifyOrderProductsStockAsync(ShopOrder order)
    {
        var result = new List<ProductsStockErrorItem>();

        foreach (var item in order.Items)
        {
            var orderLinesWithProduct = await _context.OrderLines
                .AsNoTracking()
                .Include(ol => ol.Order)
                .Where(ol => ol.ProductId == item.ProductId)
                .Where(ol => ol.Order.StatusId == OrderStatuses.Pending)
                .ToListAsync();

            var stockAvailable = item.Product.Stock - orderLinesWithProduct.Sum(ol => ol.Quantity);
            if (item.Quantity > stockAvailable)
                result.Add(new ProductsStockErrorItem(item.ProductId, item.Quantity, stockAvailable));
        }

        return result;
    }

    #endregion

    #endregion

    #region Webhook

    public async Task<Result> ProcessWebhookAsync(string payload, string signature)
    {
        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(payload, signature, _stripeSettings.WebhookSecret);
        }
        catch (StripeException ex)
        {
            _logger.LogError("Webhook signature verification failed: {Error}", ex.Message);
            return new WebhookError();
        }

        switch (stripeEvent.Type)
        {
            case EventTypes.CheckoutSessionCompleted:
            {
                var session = stripeEvent.Data.Object as Session;
                if (session == null) return new WebhookError();

                return await HandleCheckoutCompleted(session);
            }
            case EventTypes.CheckoutSessionExpired:
            {
                var session = stripeEvent.Data.Object as Session;
                if (session == null) return new WebhookError();

                return await HandleCheckoutExpired(session);
            }
            // case EventTypes.PaymentIntentPaymentFailed:
            // {
            //     var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            //     if (paymentIntent == null) return null;
            //
            //     return await HandlePaymentFailed(paymentIntent);
            // }
            default:
                return new WebhookError();
        }
    }

    private async Task<Result> HandleCheckoutCompleted(Session session)
    {
        if (!int.TryParse(session.Metadata.GetValueOrDefault("orderId"), out var orderId))
        {
            _logger.LogWarning("Could not get order id for session {SessionId}", session.Id);
            return new WebhookError();
        }

        var order = await _context.ShopOrders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            _logger.LogWarning("Order not found for session {SessionId}", session.Id);
            return new WebhookError();
        }

        if (order.StatusId == OrderStatuses.Paid)
        {
            return Result.Success();
        }

        var amount = session.AmountTotal ?? 0;

        var payment = new Payment
        {
            OrderId = order.Id,
            Order = order,
            Amount = (decimal)amount / 100,
            Currency = session.Currency,
            CreatedAt = DateTime.UtcNow,
        };

        await _context.Payments.AddAsync(payment);
        order.StatusId = OrderStatuses.Paid;

        foreach (var item in order.Items)
            item.Product.Stock = Math.Max(item.Product.Stock - item.Quantity, 0);

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Session {SessionId} successfully completed. Created payment {PaymentId} for order {OrderId}.",
            session.Id, payment.Id, payment.OrderId);

        return Result.Success();
    }

    private async Task<Result> HandleCheckoutExpired(Session session)
    {
        if (!int.TryParse(session.Metadata.GetValueOrDefault("orderId"), out var orderId))
        {
            _logger.LogWarning("Could not get order id for session {SessionId}", session.Id);
            return new WebhookError();
        }

        var order = await _context.ShopOrders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            _logger.LogWarning("Order not found for session {SessionId}", session.Id);
            return new WebhookError();
        }

        if (order.StatusId != OrderStatuses.Pending)
            return Result.Success();

        order.StatusId = OrderStatuses.Expired;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Session {SessionId} timed out. Updating order {OrderId} status to EXPIRED",
            session.Id, order.Id);

        return Result.Success();
    }

    #endregion
}