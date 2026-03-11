using ECommerce.Api.Application.DTOs.User;
using ECommerce.Api.Domain.Validation;
using ECommerce.Api.Extensions;
using FluentValidation;

namespace ECommerce.Api.Validators.DTOs;

public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateDtoValidator()
    {
        RuleFor(u => u.Password)
            .MinimumLength(TextLengthRules.PasswordMinLenght)
            .Unless(u => u.Password == null);

        RuleFor(u => u.BirthDate)
            .ParseableToDate()
            .Unless(u => u.BirthDate == null);
    }
}