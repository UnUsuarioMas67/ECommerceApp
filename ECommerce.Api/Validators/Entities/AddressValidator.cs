using ECommerce.Api.Entities;
using ECommerce.Api.Shared;
using FluentValidation;

namespace ECommerce.Api.Validators.Entities;

public class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {
        RuleFor(a => a.Id)
            .GreaterThanOrEqualTo(0);

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

        RuleFor(a => a.ClientId)
            .GreaterThan(0)
            .Unless(a => a.ClientId == null);
        
        RuleFor(a => a.CountryCca2)
            .Length(2)
            .Unless(a => string.IsNullOrEmpty(a.CountryCca2));

        RuleFor(a => a.Country)
            .NotNull();
    }
}