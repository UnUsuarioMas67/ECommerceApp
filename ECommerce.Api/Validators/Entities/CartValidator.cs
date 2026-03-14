using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Extensions;
using ECommerce.Api.Infrastructure.EF;
using FluentValidation;

namespace ECommerce.Api.Validators.Entities;

public class CartValidator : AbstractValidator<Cart>
{
    public CartValidator(IValidator<CartItem> cartItemValidator, ECommerceContext context)
    {
        Unless(c => c.ClientId == 0, () =>
        {
            RuleFor(c => c.ClientId)
                .ClientIsValid(context.Clients);
        }).Otherwise(() =>
        {
            RuleFor(c => c.Client)
                .ClientIsValid(context.Clients);
        });

        RuleFor(c => c.Items).NotEmpty();
        RuleForEach(c => c.Items).SetValidator(cartItemValidator);
    }
}