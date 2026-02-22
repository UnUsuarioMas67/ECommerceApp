using ECommerce.Api.Application.DTOs.Address;
using ECommerce.Api.Domain.Validation;
using FluentValidation;

namespace ECommerce.Api.Validators.DTOs;

public class UpdateAddressDtoValidator : AbstractValidator<UpdateAddressDto>
{
    public UpdateAddressDtoValidator()
    {
        RuleFor(a => a.AddressLine1)
            .NotEmpty().WithMessage("AddressLine1 is required")
            .Unless(a => a.AddressLine1 == null)
            .MaximumLength(TextLengthRules.ShortText);
        
        RuleFor(a => a.AddressLine2)
            .MaximumLength(TextLengthRules.ShortText);

        RuleFor(a => a.CountryCode)
            .NotEmpty().WithMessage("CountryCode is required")
            .Unless(a => a.CountryCode == null)
            .MaximumLength(2);

        RuleFor(a => a.Region)
            .MaximumLength(TextLengthRules.ShortText);

        RuleFor(a => a.City)
            .NotEmpty().WithMessage("City is required")
            .Unless(a => a.City == null)
            .MaximumLength(TextLengthRules.ShortText);

        RuleFor(a => a.PostalCode)
            .NotEmpty().WithMessage("Postal Code is required")
            .Unless(a => a.PostalCode == null)
            .MaximumLength(TextLengthRules.PostalCode);
    }
}