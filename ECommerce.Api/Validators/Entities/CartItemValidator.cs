using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Infrastructure.EF;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Validators.Entities;

public class CartItemValidator : AbstractValidator<CartItem>
{
    public CartItemValidator()
    {
        RuleFor(ci => ci.Product)
            .NotNull()
            .WithName("ProductId")
            .WithMessage("Product not specified or does not exist");

        RuleFor(ci => ci.Cart)
            .NotNull()
            .WithName("CartId")
            .WithMessage("Cart not specified or does not exist");
        
        RuleFor(ci => ci.Quantity)
            .NotNull()
            .GreaterThan(0)
            .WithMessage("Quantity cannot be negative");
    }
}