using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Infrastructure.EF;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Validators.Entities;

public class CartValidator : AbstractValidator<Cart>
{
    private readonly ECommerceContext _context;

    public CartValidator(ECommerceContext context)
    {
        _context = context;

        RuleFor(c => c.ClientId)
            .NotNull()
            .MustAsync(ClientExists)
            .WithMessage("Client does not exist");
        
        RuleForEach(c => c.Items).SetValidator(new CartItemValidator());
    }

    private async Task<bool> ClientExists(int clientId, CancellationToken token)
    {
        return await _context.Clients.AnyAsync(c => c.Id == clientId, token);
    }
}