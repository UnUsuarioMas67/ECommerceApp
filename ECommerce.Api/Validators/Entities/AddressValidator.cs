using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Domain.Validation;
using ECommerce.Api.Infrastructure.EF;
using FluentValidation;

namespace ECommerce.Api.Validators.Entities;

public class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator(ECommerceContext context)
    {
        RuleFor(a => a.AddressLine1)
            .NotEmpty()
            .MaximumLength(TextLengthRules.ShortText);

        RuleFor(a => a.AddressLine2)
            .MaximumLength(TextLengthRules.ShortText);

        RuleFor(a => a.City)
            .NotEmpty()
            .MaximumLength(TextLengthRules.ShortText);

        RuleFor(a => a.PostalCode)
            .NotEmpty()
            .MaximumLength(TextLengthRules.PostalCode);

        RuleFor(a => a.Region)
            .MaximumLength(TextLengthRules.ShortText);
        
        var countries = context.Countries;
        RuleFor(a => a.CountryId)
            .NotEmpty()
            .Must(id => countries.Any(c => c.Id == id));
    }
}