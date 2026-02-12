using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Domain.Validation;
using ECommerce.Api.Infrastructure.EF;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Validators.Entities;

public class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator(ECommerceContext context)
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(TextLengthRules.Name);

        RuleFor(c => c.Description)
            .NotEmpty()
            .MaximumLength(TextLengthRules.ShortText);

        var categories = context.Categories;
        RuleFor(c => c.Name)
            .MustAsync(async (category, name, token) =>
            {
                return !await categories.AnyAsync(c 
                    => c.Name == name && c.Id != category.Id);
            })
            .WithMessage("Category with that name already exists.");
    }
}