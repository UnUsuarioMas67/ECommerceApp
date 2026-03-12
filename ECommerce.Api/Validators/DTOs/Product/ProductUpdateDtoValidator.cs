using ECommerce.Api.Application.DTOs.Product;
using ECommerce.Api.Domain.Validation;
using FluentValidation;

namespace ECommerce.Api.Validators.DTOs.Product;

public class ProductUpdateDtoValidator : AbstractValidator<ProductUpdateDto>
{
    public ProductUpdateDtoValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .Unless(p => p.Name == null)
            .MaximumLength(TextLengthRules.ShortText);

        RuleFor(p => p.Description)
            .NotEmpty()
            .Unless(p => p.Description == null)
            .MaximumLength(TextLengthRules.LongText);

        RuleFor(p => p.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price cannot be negative");
        
        RuleFor(p => p.Category)
            .NotEmpty()
            .Unless(p => p.Category == null)
            .WithMessage("Category is required")
            .MaximumLength(TextLengthRules.Name);
    }
}