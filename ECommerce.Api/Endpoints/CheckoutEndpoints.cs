using ECommerce.Api.Application.DTOs.Checkout;
using ECommerce.Api.Errors;
using ECommerce.Api.Services.Payment;
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
            .RequireAuthorization();

        group.MapPost("/webhook", ProcessWebhook)
            .WithSummary("Stripe Webhook")
            .AllowAnonymous();

        return endpoints;
    }

    private static async Task<Results<Ok<CheckoutResponseDto>, ValidationProblem, UnprocessableEntity<Error>,
            BadRequest<InvalidAuthenticationError>>> 
        CreateCheckoutSession(
            HttpContext context,
            [FromBody] CheckoutRequestDto request,
            IValidator<CheckoutRequestDto> validator,
            IStripeCheckoutService stripeService)
    {
        var clientId = AuthUser.GetAuthUserId(context);
        if (!clientId.HasValue)
            return TypedResults.BadRequest(new InvalidAuthenticationError());

        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await stripeService.CreateCheckoutSessionAsync(request, clientId.Value);
        if (result.IsSuccess)
            return TypedResults.Ok(result.Value);

        return TypedResults.UnprocessableEntity(result.Error);
    }

    private static async Task<Results<Ok<PaymentResultDto>, NotFound>> GetPaymentBySession(
        string sessionId,
        IStripeCheckoutService stripeService)
    {
        var payment = await stripeService.GetPaymentBySessionIdAsync(sessionId);
        return payment != null ? TypedResults.Ok(payment) : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<PaymentResultDto>, BadRequest<Error>>> ProcessWebhook(
        HttpContext context,
        IStripeCheckoutService stripeService)
    {
        using var reader = new StreamReader(context.Request.Body);
        var payload = await reader.ReadToEndAsync();

        var signature = context.Request.Headers["Stripe-Signature"].FirstOrDefault() ?? string.Empty;

        if (string.IsNullOrEmpty(signature))
            return TypedResults.BadRequest(new Error("Missing Stripe-Signature header"));

        var result = await stripeService.ProcessWebhookAsync(payload, signature);

        if (result == null)
            return TypedResults.BadRequest(new Error("Webhook processing failed"));

        return TypedResults.Ok(result);
    }
}