using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Domain.Validation;
using FluentValidation;

namespace ECommerce.Api.Validators.Entities;

public class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {
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
        
        When(a => a.Country == null, () =>
        {
            RuleFor(a => a.CountryCca2)
                .NotEmpty()
                .MaximumLength(2)
                .WithMessage("Country is required");
        });

        When(a => string.IsNullOrWhiteSpace(a.CountryCca2), () =>
        {
            RuleFor(a => a.Country)
                .NotNull()
                .WithMessage("Country is required");
        });
    }
}