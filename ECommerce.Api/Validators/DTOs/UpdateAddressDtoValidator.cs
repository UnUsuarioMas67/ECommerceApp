using ECommerce.Api.Application.DTOs.Address;
using ECommerce.Api.Domain.Validation;
using FluentValidation;

namespace ECommerce.Api.Validators.DTOs;

public class UpdateAddressDtoValidator : AbstractValidator<UpdateAddressDto>
{
    public UpdateAddressDtoValidator()
    {
        RuleFor(a => a.AddressLine1)
            .NotEmpty()
            .Unless(a => a.AddressLine1 == null)
            .MaximumLength(TextLengthRules.ShortText);

        RuleFor(a => a.AddressLine2)
            .MaximumLength(TextLengthRules.ShortText);

        RuleFor(a => a.CountryCode)
            .NotEmpty()
            .Unless(a => a.CountryCode == null)
            .MaximumLength(2);

        RuleFor(a => a.Region)
            .NotEmpty()
            .Unless(a => a.Region == null)
            .MaximumLength(TextLengthRules.ShortText);

        RuleFor(a => a.City)
            .NotEmpty()
            .Unless(a => a.City == null)
            .MaximumLength(TextLengthRules.ShortText);

        RuleFor(a => a.PostalCode)
            .NotEmpty()
            .Unless(a => a.PostalCode == null)
            .MaximumLength(TextLengthRules.PostalCode);
    }
}