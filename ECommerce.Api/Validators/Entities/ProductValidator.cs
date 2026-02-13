using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Domain.Validation;
using ECommerce.Api.Infrastructure.EF;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Validators.Entities;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator(ECommerceContext context)
    {
        RuleFor(p => p.Sku)
            .NotEmpty()
            .MaximumLength(TextLengthRules.Sku);
        
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(TextLengthRules.ShortText);
        
        RuleFor(p => p.Description)
            .NotEmpty()
            .MaximumLength(TextLengthRules.LongText);

        RuleFor(p => p.Price)
            .NotNull()
            .GreaterThanOrEqualTo(0);
        
        RuleFor(p => p.Stock)
            .NotNull()
            .GreaterThanOrEqualTo(0);
        
        var categories = context.Categories;
        RuleFor(p => p.CategoryId)
            .NotNull()
            .MustAsync(async (id, token) => await categories.AnyAsync(c => c.Id == id))
            .WithMessage("Category doesn't exist");
    }
}