using System.Diagnostics.CodeAnalysis;
using ECommerce.Api.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Domain.Validation;

public class ClientValidator : AbstractValidator<Client>
{
    public ClientValidator(DbSet<Client> clients)
    {
        RuleFor(c => c.FirstName)
            .NotEmpty()
            .MaximumLength(TextLengthRules.Name);

        RuleFor(c => c.LastName)
            .NotEmpty()
            .MaximumLength(TextLengthRules.Name);

        RuleFor(c => c.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(TextLengthRules.Email);

        RuleFor(c => c.PhoneNumber)
            .NotEmpty()
            .MaximumLength(TextLengthRules.PhoneNumber);

        RuleFor(c => c.BirthDate)
            .NotNull()
            .Must(date => date <= DateOnly.FromDateTime(DateTime.Today));

        RuleFor(c => c.CreatedAt)
            .NotNull()
            .Must(date => date <= DateTime.UtcNow);

        RuleFor(c => c.PasswordHash)
            .NotEmpty();

        RuleSet("AddNew", () =>
        {
            RuleFor(c => c.Id).Equal(0);

            RuleFor(c => c.Email)
                .MustAsync(async (email, token)
                    => !await clients.AnyAsync(c => c.Email == email));

            RuleFor(c => c.PhoneNumber)
                .MustAsync(async (phoneNumber, token)
                    => !await clients.AnyAsync(c => c.PhoneNumber == phoneNumber));
        });
    }
}