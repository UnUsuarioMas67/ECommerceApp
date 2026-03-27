using ECommerce.Api.Application.DTOs.Checkout;
using FluentValidation;

namespace ECommerce.Api.Validators.DTOs.Checkout;

public class CheckoutRequestDtoValidator : AbstractValidator<CheckoutRequestDto>
{
    public CheckoutRequestDtoValidator()
    {
        RuleFor(r => r.CartId)
            .GreaterThan(0)
            .NotNull();

        RuleFor(r => r.AddressId)
            .GreaterThan(0)
            .NotNull();
        
        RuleFor(r => r.SuccessUrl)
            .NotEmpty()
            .WithMessage("SuccessUrl is required");
    }
}