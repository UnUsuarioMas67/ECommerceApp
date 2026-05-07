using ECommerce.Api.DTOs.Checkout;
using ECommerce.Api.DTOs.Error;
using ECommerce.Api.Errors;
using ECommerce.Api.Services.Checkout;
using ECommerce.Api.Services.DataAccess;
using ECommerce.Api.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Endpoints;

public static class CheckoutEndpoints
{
    public static IEndpointRouteBuilder MapCheckoutEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/checkout")
            .WithTags("Checkout");

        group.MapPost("/session", CreateCheckoutSession)
            .WithSummary("Create a Stripe Checkout Session")
            .RequireAuthorization(UserRoles.Client);

        group.MapGet("/payment/{sessionId}", GetPaymentBySession)
            .WithSummary("Get Payment by Session")
            .RequireAuthorization(UserRoles.Client);

        group.MapPost("/webhook", ProcessWebhook)
            .WithSummary("Stripe Webhook")
            .AllowAnonymous();

        return endpoints;
    }

    private static async Task<Results<Ok<CheckoutResponseDto>, BadRequest<ErrorDto>, UnprocessableEntity<ErrorDto>,
            UnauthorizedHttpResult>> 
        CreateCheckoutSession(
            HttpContext context,
            [FromBody] CheckoutRequestDto request,
            IValidator<CheckoutRequestDto> validator,
            IStripeCheckoutService stripeService)
    {
        var clientId = AuthUser.GetAuthUserId(context);
        if (!clientId.HasValue)
            return TypedResults.Unauthorized();

        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return TypedResults.BadRequest(new ValidationError(validation.ToDictionary()).ToDto());

        var result = await stripeService.CreateCheckoutSessionAsync(request, clientId.Value);
        if (result.IsSuccess)
            return TypedResults.Ok(result.Value);

        return TypedResults.UnprocessableEntity(result.Error.ToDto());
    }
    
    private static async Task<Results<Ok<PaymentResultDto>, NotFound, UnauthorizedHttpResult>> GetPaymentBySession(
        HttpContext context,
        string sessionId,
        IPaymentService paymentService)
    {
        var clientId = AuthUser.GetAuthUserId(context);
        if (clientId == null)
            return TypedResults.Unauthorized();
        
        var payment = await paymentService.GetPaymentBySessionIdAsync(sessionId, clientId.Value);
        return payment != null ? TypedResults.Ok(payment) : TypedResults.NotFound();
    }

    private static async Task<Results<NoContent, BadRequest>> ProcessWebhook(
        HttpContext context,
        IStripeCheckoutService stripeService)
    {
        using var reader = new StreamReader(context.Request.Body);
        var payload = await reader.ReadToEndAsync();

        var signature = context.Request.Headers["Stripe-Signature"].FirstOrDefault() ?? string.Empty;

        if (string.IsNullOrEmpty(signature))
            return TypedResults.BadRequest();

        var result = await stripeService.ProcessWebhookAsync(payload, signature);

        if (!result.IsSuccess)
            return TypedResults.BadRequest();

        return TypedResults.NoContent();
    }
}