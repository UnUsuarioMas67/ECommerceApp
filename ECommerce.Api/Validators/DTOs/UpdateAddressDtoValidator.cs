using ECommerce.Api.Application.DTOs.Address;
using ECommerce.Api.Domain.Validation;
using FluentValidation;

namespace ECommerce.Api.Validators.DTOs;

public class UpdateAddressDtoValidator : AbstractValidator<UpdateAddressDto>
{
    public UpdateAddressDtoValidator()
    {
        RuleFor(a => a.CountryCode)
            .NotEmpty()
            .Unless(a => a.CountryCode == null)
            .MaximumLength(2);
    }
}