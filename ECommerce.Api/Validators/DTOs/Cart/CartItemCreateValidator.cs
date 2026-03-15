using ECommerce.Api.Application.DTOs.Cart;
using FluentValidation;

namespace ECommerce.Api.Validators.DTOs.Cart;

public class CartItemCreateValidator : AbstractValidator<CartItemCreate>
{
    public CartItemCreateValidator()
    {
        RuleFor(ci => ci.ProductId)
            .GreaterThan(0);
        
        RuleFor(ci => ci.Quantity)
            .GreaterThan(0);
    }
}