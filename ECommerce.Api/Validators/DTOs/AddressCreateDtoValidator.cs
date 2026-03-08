using ECommerce.Api.Application.DTOs.Address;
using FluentValidation;

namespace ECommerce.Api.Validators.DTOs;

public class AddressCreateDtoValidator : AbstractValidator<AddressCreateDto>
{
    public AddressCreateDtoValidator()
    {
        RuleFor(a => a.ClientId)
            .NotEmpty();

        RuleFor(a => a.CountryCode)
            .NotEmpty()
            .MaximumLength(2);
    }
}