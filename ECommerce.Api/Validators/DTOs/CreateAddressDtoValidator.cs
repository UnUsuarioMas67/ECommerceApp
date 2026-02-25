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

        RuleFor(a => a.CountryCode)
            .NotEmpty()
            .MaximumLength(2);
    }
}