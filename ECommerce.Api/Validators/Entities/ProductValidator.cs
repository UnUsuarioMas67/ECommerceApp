using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Domain.Validation;
using FluentValidation;
using FluentValidation.Results;

namespace ECommerce.Api.Validators.Entities;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
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
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price cannot be negative");
        
        RuleFor(p => p.Stock)
            .NotNull()
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock cannot be negative");
    }
}