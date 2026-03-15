using ECommerce.Api.Application.DTOs.Address;
using ECommerce.Api.Domain.Validation;
using FluentValidation;

namespace ECommerce.Api.Validators.DTOs.Address;

public class AddressUpdateDtoValidator : AbstractValidator<AddressUpdateDto>
{
    public AddressUpdateDtoValidator()
    {
        RuleFor(p => p.AddressLine1)
            .NotEmpty()
            .Unless(p => p.AddressLine1 == null)
            .MaximumLength(TextLengthRules.ShortText);
        
        RuleFor(p => p.AddressLine2)
            .MaximumLength(TextLengthRules.ShortText);
        
        RuleFor(p => p.PostalCode)
            .NotEmpty()
            .Unless(p => p.PostalCode == null)
            .MaximumLength(TextLengthRules.PostalCode);
        
        RuleFor(p => p.Region)
            .NotEmpty()
            .Unless(p => p.Region == null)
            .MaximumLength(TextLengthRules.ShortText);
        
        RuleFor(p => p.City)
            .NotEmpty()
            .Unless(p => p.City == null)
            .MaximumLength(TextLengthRules.ShortText);
        
        RuleFor(a => a.CountryCode)
            .NotEmpty()
            .Unless(a => a.CountryCode == null)
            .Length(2);
    }
}