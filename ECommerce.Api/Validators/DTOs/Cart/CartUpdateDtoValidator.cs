using ECommerce.Api.Application.DTOs.Cart;
using FluentValidation;
using FluentValidation.Validators;

namespace ECommerce.Api.Validators.DTOs.Cart;

public class CartUpdateDtoValidator : AbstractValidator<CartUpdateDto>
{
    public CartUpdateDtoValidator(IValidator<CartItemCreate> cartItemValidator)
    {
        RuleFor(c => c.Items)
            .NotEmpty();

        RuleForEach(c => c.Items)
            .SetValidator(cartItemValidator);
    }
}