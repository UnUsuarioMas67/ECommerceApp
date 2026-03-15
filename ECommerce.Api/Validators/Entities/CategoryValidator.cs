using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Domain.Validation;
using ECommerce.Api.Extensions;
using ECommerce.Api.Infrastructure.EF;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

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

    public static IRuleBuilderOptions<Category, string> SlugIsUnique(
        this IRuleBuilder<Category, string> ruleBuilder, DbSet<Category> categories)
    {
        return ruleBuilder
            .MustAsync(async (slug, token) 
                => !await categories.AnyAsync(c => c.Slug == slug, token))
            .WithMessage("A category with the same slug already exists.");
    }
}