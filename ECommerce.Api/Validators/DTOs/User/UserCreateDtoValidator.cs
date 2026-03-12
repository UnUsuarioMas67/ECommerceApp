using ECommerce.Api.Application.DTOs.User;
using ECommerce.Api.Domain.Validation;
using ECommerce.Api.Extensions;
using FluentValidation;

namespace ECommerce.Api.Validators.DTOs.User;

public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateDtoValidator()
    {
        RuleFor(u => u.Password)
            .NotEmpty()
            .MinimumLength(TextLengthRules.PasswordMinLenght);

        RuleFor(u => u.Email)
            .EmailAddress()
            .MaximumLength(TextLengthRules.Email);

        RuleFor(u => u.BirthDate)
            .NotNull()
            .ParseableToDate();
    }
}