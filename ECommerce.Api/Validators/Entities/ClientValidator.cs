using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Domain.Validation;
using ECommerce.Api.Extensions;
using ECommerce.Api.Infrastructure.EF;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Validators.Entities;

public class ClientValidator : AbstractValidator<Client>
{
    public ClientValidator(ECommerceContext context)
    {
        RuleFor(u => u.Id)
            .IdIsDefaultOnNewEntry(context.Entry);
        
        RuleFor(u => u.FirstName)
            .NotEmpty()
            .MaximumLength(TextLengthRules.Name);

        RuleFor(u => u.LastName)
            .NotEmpty()
            .MaximumLength(TextLengthRules.Name);

        RuleFor(u => u.BirthDate)
            .NotNull()
            .NotInTheFuture();

        RuleFor(u => u.CreatedAt)
            .NotNull()
            .NotInTheFuture();

        RuleFor(u => u.PasswordHash)
            .NotEmpty();

        RuleFor(u => u.Email)
            .NotEmpty()
            .MaximumLength(TextLengthRules.Email)
            .EmailNotDuplicate(context.Clients);

        RuleFor(u => u.PhoneNumber)
            .NotEmpty()
            .MaximumLength(TextLengthRules.PhoneNumber)
            .PhoneNumberNotDuplicate(context.Clients);
    }
}

public static class ClientValidationExtensions
{
    public static IRuleBuilderOptions<Client, string> EmailNotDuplicate(
        this IRuleBuilder<Client, string> ruleBuilder, DbSet<Client> clients)
    {
        return ruleBuilder
            .MustAsync(async (client, email, token) =>
                !await clients.AnyAsync(c => c.Email == email && c.Id != client.Id, token))
            .WithMessage("A client with that email already exists.");
    }

    public static IRuleBuilderOptions<Client, string> PhoneNumberNotDuplicate(
        this IRuleBuilder<Client, string> ruleBuilder, DbSet<Client> clients)
    {
        return ruleBuilder
            .MustAsync(async (client, phoneNumber, token) =>
                !await clients.AnyAsync(c => c.PhoneNumber == phoneNumber && c.Id != client.Id, token))
            .WithMessage("This phone number is already in use.");
    }
}