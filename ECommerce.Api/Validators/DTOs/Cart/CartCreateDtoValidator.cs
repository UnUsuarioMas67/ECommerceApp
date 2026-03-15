using ECommerce.Api.Application.DTOs.Cart;
using FluentValidation;

namespace ECommerce.Api.Validators.DTOs.Cart;

public class CartCreateDtoValidator : AbstractValidator<CartCreateDto>
{
    public CartCreateDtoValidator(IValidator<CartItemCreate> cartItemValidator)
    {
        RuleFor(c => c.ClientId)
            .GreaterThan(0);

        RuleFor(c => c.Items)
            .NotEmpty();

        RuleForEach(c => c.Items)
            .SetValidator(cartItemValidator);
    }
}