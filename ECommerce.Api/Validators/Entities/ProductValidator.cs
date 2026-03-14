using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Domain.Validation;
using ECommerce.Api.Extensions;
using ECommerce.Api.Infrastructure.EF;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Validators.Entities;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator(ECommerceContext context, IValidator<Category> categoryValidator)
    {
        RuleFor(p => p.Id)
            .IdIsDefaultOnNewEntry(context.Entry);
        
        RuleFor(p => p.Sku)
            .NotEmpty()
            .SkuIsUnique(context.Products)
            .MaximumLength(TextLengthRules.Sku);

        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(TextLengthRules.ShortText);

        RuleFor(p => p.Description)
            .NotEmpty()
            .MaximumLength(TextLengthRules.LongText);

        RuleFor(p => p.Price)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price cannot be negative");

        RuleFor(p => p.Stock)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock cannot be negative");

        Unless(p => p.CategoryId == null, () =>
        {
            RuleFor(p => p.CategoryId)
                .CategoryIsValid(context.Categories);
        })
        .Otherwise(() =>
        {
            RuleFor(p => p.Category)
                .CategoryIsValid(context.Categories);
        });
    }
}

public static class ProductValidationExtensions
{
    public static IRuleBuilderOptions<Product, int?> CategoryIsValid(
        this IRuleBuilder<Product, int?> ruleBuilder, DbSet<Category> categories)
    {
        return ruleBuilder
            .GreaterThan(0)
            .MustAsync(async (categoryId, token) =>
            {
                if (categoryId == null)
                    return true;
                
                return await categories.AnyAsync(c => c.Id == categoryId, token);
            })
            .WithMessage("The specified category doesn't seem to exist");
    }

    public static IRuleBuilderOptions<Product, Category?> CategoryIsValid(
        this IRuleBuilder<Product, Category?> ruleBuilder, DbSet<Category> categories)
    {
        return ruleBuilder
            .MustAsync(async (category, token) =>
            {
                if (category == null)
                    return true;
                return await categories.AnyAsync(c => c == category, token);
            })
            .WithMessage("The specified category doesn't seem to exist");
    }
    
    public static IRuleBuilderOptions<Product, string> SkuIsUnique(
        this IRuleBuilder<Product, string> ruleBuilder, DbSet<Product> products)
    {
        return ruleBuilder
            .MustAsync(async (product, sku, token) 
                => !await products.AnyAsync(p => p.Sku == sku && p != product, token))
            .WithMessage("A product with the same SKU already exists.");
    }
}