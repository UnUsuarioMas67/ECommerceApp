using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Domain.Validation;
using FluentValidation;

namespace ECommerce.Api.Validators.Entities;

public class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(c => c.Slug)
            .NotEmpty()
            .Must(slug => !double.TryParse(slug, out _))
            .WithMessage("Slug must not be a number.")
            .MaximumLength(TextLengthRules.Name);
        
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(TextLengthRules.Name);
    }
}