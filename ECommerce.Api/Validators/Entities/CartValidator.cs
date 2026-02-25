using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Infrastructure.EF;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Validators.Entities;

public class CartValidator : AbstractValidator<Cart>
{
    public CartValidator()
    {
        When(c => c.Client == null, () =>
        {
            RuleFor(c => c.ClientId)
                .GreaterThan(0)
                .WithMessage("Client is required");
        });

        When(c => c.ClientId <= 0, () =>
        {
            RuleFor(c => c.Client)
                .NotNull()
                .WithMessage("Client is required");
        });
        
        RuleForEach(c => c.Items).SetValidator(new CartItemValidator());
    }
}