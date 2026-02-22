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
            .NotEmpty().WithMessage("AddressLine2 cannot be blank text")
            .Unless(a => a.AddressLine2 == null)
            .MaximumLength(TextLengthRules.ShortText);

        RuleFor(a => a.City)
            .NotEmpty()
            .MaximumLength(TextLengthRules.ShortText);

        RuleFor(a => a.PostalCode)
            .NotEmpty()
            .MaximumLength(TextLengthRules.PostalCode);

        RuleFor(a => a.Region)
            .NotEmpty().WithMessage("Region cannot be blank text")
            .Unless(a => a.Region == null)
            .MaximumLength(TextLengthRules.ShortText);
        
        var countries = context.Countries;
        RuleFor(a => a.CountryId)
            .NotEmpty()
            .Must(id => countries.Any(c => c.Id == id))
            .WithMessage("The specified country does not exist.");
    }
}