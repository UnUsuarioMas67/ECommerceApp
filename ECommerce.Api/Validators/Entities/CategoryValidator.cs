using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Domain.Validation;
using ECommerce.Api.Infrastructure.EF;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Validators.Entities;

public class CategoryValidator : AbstractValidator<Category>
{
    private readonly DbSet<Category> categories;
    
    public CategoryValidator(ECommerceContext context)
    {
        categories = context.Categories;
        
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(TextLengthRules.Name);

        RuleFor(c => c.Description)
            .NotEmpty()
            .MaximumLength(TextLengthRules.ShortText);

        RuleFor(c => c.Name)
            .MustAsync(NameIsUnique)
            .WithMessage("Category with that name already exists.");
    }

    private async Task<bool> NameIsUnique(Category category, string name, CancellationToken token)
    {
        return !await categories.AnyAsync(c 
            => c.Name == name && c.Id != category.Id);
    }
}