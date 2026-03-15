using ECommerce.Api.Application.DTOs.Address;
using ECommerce.Api.Domain.Validation;
using FluentValidation;

namespace ECommerce.Api.Validators.DTOs.Address;

public class AddressCreateDtoValidator : AbstractValidator<AddressCreateDto>
{
    public AddressCreateDtoValidator()
    {
        RuleFor(a => a.ClientId)
            .GreaterThan(0);
        
        RuleFor(p => p.AddressLine1)
            .NotEmpty()
            .MaximumLength(TextLengthRules.ShortText);
        
        RuleFor(p => p.AddressLine2)
            .MaximumLength(TextLengthRules.ShortText);
        
        RuleFor(p => p.PostalCode)
            .NotEmpty()
            .MaximumLength(TextLengthRules.PostalCode);
        
        RuleFor(p => p.Region)
            .NotEmpty()
            .MaximumLength(TextLengthRules.ShortText);
        
        RuleFor(p => p.City)
            .NotEmpty()
            .MaximumLength(TextLengthRules.ShortText);
        
        RuleFor(a => a.CountryCode)
            .NotEmpty()
            .Length(2);
    }
}