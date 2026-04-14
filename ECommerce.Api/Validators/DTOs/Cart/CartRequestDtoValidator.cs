using ECommerce.Api.DTOs.Cart;
using FluentValidation;

namespace ECommerce.Api.Validators.DTOs.Cart;

public class CartRequestDtoValidator : AbstractValidator<CartRequestDto>
{
    public CartRequestDtoValidator(IValidator<CartItemCreate> cartItemValidator)
    {
        RuleForEach(c => c.Items)
            .SetValidator(cartItemValidator);
    }
}