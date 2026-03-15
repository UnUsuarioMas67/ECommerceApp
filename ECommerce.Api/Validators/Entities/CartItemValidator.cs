using ECommerce.Api.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

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

public static class CartItemValidationExtensions
{
    public static IRuleBuilderOptions<CartItem, int> ProductIsValid(
        this IRuleBuilder<CartItem, int> ruleBuilder, DbSet<Product> products)
    {
        return ruleBuilder
            .GreaterThan(0)
            .NotEmpty().WithMessage("{PropertyName} is required")
            .MustAsync(async (productId, token) => await products.AnyAsync(c => c.Id == productId, token))
            .WithMessage("The specified product doesn't seem to exist");
    }

    public static IRuleBuilderOptions<CartItem, Product> ProductIsValid(
        this IRuleBuilder<CartItem, Product> ruleBuilder, DbSet<Product> products)
    {
        return ruleBuilder
            .NotNull().WithMessage("{PropertyName} is required")
            .MustAsync(async (product, token) => await products.AnyAsync(c => c == product, token))
            .WithMessage("The specified product doesn't seem to exist");
    }
}