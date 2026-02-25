using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Infrastructure.EF;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Validators.Entities;

public class CartItemValidator : AbstractValidator<CartItem>
{
    public CartItemValidator()
    {
        When(ci => ci.Product == null, () =>
        {
            RuleFor(ci => ci.ProductId)
                .GreaterThan(0)
                .WithMessage("Product is required");
        });

        When(ci => ci.ProductId <= 0, () =>
        {
            RuleFor(ci => ci.Product)
                .NotNull()
                .WithMessage("Product is required");
        });
        
        When(ci => ci.Cart == null, () =>
        {
            RuleFor(ci => ci.CartId)
                .GreaterThan(0)
                .WithMessage("Cart is required");
        });

        When(ci => ci.CartId <= 0, () =>
        {
            RuleFor(ci => ci.Cart)
                .NotNull()
                .WithMessage("Cart is required");
        });
        
        RuleFor(ci => ci.Quantity)
            .NotNull()
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero");
    }
}