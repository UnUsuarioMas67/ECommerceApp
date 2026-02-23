using ECommerce.Api.Application.DTOs.Address;
using ECommerce.Api.Domain.Validation;
using FluentValidation;

namespace ECommerce.Api.Validators.DTOs;

public class CreateAddressDtoValidator : AbstractValidator<CreateAddressDto>
{
    public CreateAddressDtoValidator()
    {
        RuleFor(a => a.ClientId)
            .NotEmpty();
        
        RuleFor(a => a.AddressLine1)
            .NotEmpty()
            .MaximumLength(TextLengthRules.ShortText);
        
        RuleFor(a => a.AddressLine2)
            .MaximumLength(TextLengthRules.ShortText);

        RuleFor(a => a.CountryCode)
            .NotEmpty()
            .MaximumLength(2);

        RuleFor(a => a.Region)
            .NotEmpty()
            .MaximumLength(TextLengthRules.ShortText);
        
        RuleFor(a => a.City)
            .NotEmpty()
            .MaximumLength(TextLengthRules.ShortText);

        RuleFor(a => a.PostalCode)
            .NotEmpty()
            .MaximumLength(TextLengthRules.PostalCode);
    }
}