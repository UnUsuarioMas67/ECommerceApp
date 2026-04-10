using ECommerce.Api.Entities;
using ECommerce.Api.Shared;
using FluentValidation;

namespace ECommerce.Api.Validators.Entities;

public class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(c => c.Id)
            .GreaterThanOrEqualTo(0);
        
        RuleFor(c => c.Slug)
            .NotEmpty()
            .NotANumber()
            .MaximumLength(TextLengthRules.Name);

        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(TextLengthRules.Name);
    }
}

public static class CategoryValidationExtensions
{
    public static IRuleBuilderOptions<Category, string> NotANumber(this IRuleBuilder<Category, string> ruleBuilder)
    {
        return ruleBuilder
            .Must(slug => !double.TryParse(slug, out _))
            .WithMessage("{PropertyName} must not be a number.");
    }
}