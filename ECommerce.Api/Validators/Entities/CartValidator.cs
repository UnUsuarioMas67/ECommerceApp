using ECommerce.Api.Domain.Entities;
using FluentValidation;

namespace ECommerce.Api.Validators.Entities;

public class CartValidator : AbstractValidator<Cart>
{
    public CartValidator(IValidator<CartItem> cartItemValidator)
    {
        RuleFor(c => c.Id)
            .GreaterThanOrEqualTo(0);

        RuleFor(c => c.ClientId)
            .GreaterThan(0);

        RuleFor(c => c.Client)
            .NotNull();

        RuleFor(c => c.Items)
            .NoDuplicateItems();
        
        RuleForEach(c => c.Items).SetValidator(cartItemValidator);
    }
}

public static class CartValidationExtension
{
    public static IRuleBuilderOptions<Cart, ICollection<CartItem>> NoDuplicateItems(
        this IRuleBuilder<Cart, ICollection<CartItem>> ruleBuilder)
    {
        return ruleBuilder.Must(items
            => items.Count == items.DistinctBy(i => i.ProductId).Count())
            .WithMessage("Cannot have duplicate items");
    }
}