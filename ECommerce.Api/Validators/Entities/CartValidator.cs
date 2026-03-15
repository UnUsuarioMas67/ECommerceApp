using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Extensions;
using ECommerce.Api.Infrastructure.EF;
using FluentValidation;

namespace ECommerce.Api.Validators.Entities;

public class CartValidator : AbstractValidator<Cart>
{
    public CartValidator(IValidator<CartItem> cartItemValidator, ECommerceContext context)
    {
        RuleFor(c => c.Id)
            .IdIsDefaultOnNewEntry(context.Entry);
        
        Unless(c => c.ClientId == 0, () =>
        {
            RuleFor(c => c.ClientId)
                .ClientIsValid(context.Clients);
        }).Otherwise(() =>
        {
            RuleFor(c => c.Client)
                .ClientIsValid(context.Clients);
        });

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
            => items.Count == items.DistinctBy(i => new { i.CartId, i.ProductId }).Count())
            .WithMessage("Cannot have duplicate items");
    }
}