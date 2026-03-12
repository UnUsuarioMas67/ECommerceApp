using ECommerce.Api.Application.DTOs.Product;
using ECommerce.Api.Domain.Validation;
using FluentValidation;

namespace ECommerce.Api.Validators.DTOs.Product;

public class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
{
    public ProductCreateDtoValidator()
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
        
        RuleFor(p => p.Category)
            .NotEmpty()
            .WithMessage("Category is required")
            .MaximumLength(TextLengthRules.Name);

        RuleFor(p => p.InitialStock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock cannot be negative");
    }
}