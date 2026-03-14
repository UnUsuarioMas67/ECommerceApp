using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Domain.Validation;
using ECommerce.Api.Extensions;
using ECommerce.Api.Infrastructure.EF;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Validators.Entities;

public class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator(ECommerceContext context)
    {
        RuleFor(a => a.Id)
            .IdIsDefaultOnNewEntry(context.Entry);

        RuleFor(a => a.AddressLine1)
            .NotEmpty()
            .MaximumLength(TextLengthRules.ShortText);

        RuleFor(a => a.AddressLine2)
            .NotNull()
            .MaximumLength(TextLengthRules.ShortText);

        RuleFor(a => a.City)
            .NotEmpty()
            .MaximumLength(TextLengthRules.ShortText);

        RuleFor(a => a.PostalCode)
            .NotEmpty()
            .MaximumLength(TextLengthRules.PostalCode);

        RuleFor(a => a.Region)
            .NotEmpty()
            .MaximumLength(TextLengthRules.ShortText);

        Unless(a => a.ClientId == null, () =>
            {
                RuleFor(a => a.ClientId)
                    .ClientIsValid(context.Clients);
            })
            .Otherwise(() =>
            {
                RuleFor(a => a.Client)
                    .ClientIsValid(context.Clients, true);
            });

        Unless(a => string.IsNullOrWhiteSpace(a.CountryCca2), () =>
            {
                RuleFor(a => a.CountryCca2)
                    .CountryExists(context.Countries);
            })
            .Otherwise(() =>
            {
                RuleFor(a => a.Country)
                    .CountryExists(context.Countries);
            });
    }
}

public static class AddressValidationExtensions
{
    public static IRuleBuilderOptions<Address, Country?> CountryExists(
        this IRuleBuilder<Address, Country?> ruleBuilder, DbSet<Country> countries)
    {
        return ruleBuilder
            .NotNull()
            .MustAsync(async (country, token)
                => await countries.AnyAsync(c => country != null && c == country, token))
            .WithMessage("The specified country doesn't seem to exist");
    }

    public static IRuleBuilderOptions<Address, string> CountryExists(
        this IRuleBuilder<Address, string> ruleBuilder, DbSet<Country> countries)
    {
        return ruleBuilder
            .NotEmpty()
            .MaximumLength(2)
            .MustAsync(async (cca2, token)
                => await countries.AnyAsync(c => c.Cca2 == cca2, token))
            .WithMessage("Invalid country code");
    }
}