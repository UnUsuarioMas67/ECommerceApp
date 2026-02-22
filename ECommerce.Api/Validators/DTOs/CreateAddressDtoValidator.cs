using ECommerce.Api.Application.DTOs.Address;
using ECommerce.Api.Domain.Validation;
using FluentValidation;

namespace ECommerce.Api.Validators.DTOs;

public class CreateAddressDtoValidator : AbstractValidator<CreateAddressDto>
{
    public CreateAddressDtoValidator()
    {
        RuleFor(a => a.ClientId)
            .NotEmpty().WithMessage("ClientId is required");
        
        RuleFor(a => a.AddressLine1)
            .NotEmpty().WithMessage("AddressLine1 is required")
            .MaximumLength(TextLengthRules.ShortText);
        
        RuleFor(a => a.AddressLine2)
            .MaximumLength(TextLengthRules.ShortText);

        RuleFor(a => a.CountryCode)
            .NotEmpty().WithMessage("CountryCode is required")
            .MaximumLength(2);

        RuleFor(a => a.Region)
            .MaximumLength(TextLengthRules.ShortText);
        
        RuleFor(a => a.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(TextLengthRules.ShortText);

        RuleFor(a => a.PostalCode)
            .NotEmpty().WithMessage("Postal Code is required")
            .MaximumLength(TextLengthRules.PostalCode);
    }
}