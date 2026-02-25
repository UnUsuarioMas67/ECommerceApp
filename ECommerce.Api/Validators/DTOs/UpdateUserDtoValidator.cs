using ECommerce.Api.Application.DTOs.User;
using ECommerce.Api.Domain.Validation;
using ECommerce.Api.Extensions;
using FluentValidation;

namespace ECommerce.Api.Validators.DTOs;

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(u => u.Password)
            .MinimumLength(TextLengthRules.PasswordMinLenght)
            .Unless(u => u.Password == null);

        RuleFor(u => u.BirthDate)
            .CanParseIntoDate()
            .Unless(u => u.BirthDate == null);
    }
}