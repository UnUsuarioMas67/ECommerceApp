using ECommerce.Api.Entities;
using FluentValidation;

namespace ECommerce.Api.Validators.Entities;

public class CartItemValidator : AbstractValidator<CartItem>
{
    public CartItemValidator()
    {
        RuleFor(ci => ci.ProductId)
            .GreaterThan(0);

        RuleFor(ci => ci.Product)
            .NotNull();

        RuleFor(ci => ci.CartId)
            .GreaterThanOrEqualTo(0);
        
        RuleFor(ci => ci.Cart)
            .NotNull();
        
        RuleFor(ci => ci.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero");
    }
}