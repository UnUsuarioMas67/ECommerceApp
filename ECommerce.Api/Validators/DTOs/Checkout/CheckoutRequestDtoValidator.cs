using ECommerce.Api.DTOs.Checkout;
using ECommerce.Api.Validators.DTOs.Cart;
using FluentValidation;

namespace ECommerce.Api.Validators.DTOs.Checkout;

public class CheckoutRequestDtoValidator : AbstractValidator<CheckoutRequestDto>
{
    public CheckoutRequestDtoValidator(CartRequestDtoValidator cartRequestDtoValidator)
    {
        RuleFor(r => r.CartId)
            .NotNull()
            .When(r => r.Cart == null)
            .WithMessage("Either CartId or Cart must be included in the request");

        RuleFor(r => r.CartId)
            .GreaterThan(0)
            .When(r => r.CartId != null);
        
        RuleFor(r => r.Cart)
            .SetValidator(cartRequestDtoValidator!)
            .When(r => r.Cart != null);

        RuleFor(r => r.AddressId)
            .GreaterThan(0)
            .NotNull();

        RuleFor(r => r.SuccessUrl)
            .NotEmpty()
            .WithMessage("SuccessUrl is required");
    }
}