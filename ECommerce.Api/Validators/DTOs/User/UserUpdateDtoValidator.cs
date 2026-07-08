using ECommerce.Api.DTOs.User;
using ECommerce.Api.Extensions;
using ECommerce.Api.Shared;
using FluentValidation;

namespace ECommerce.Api.Validators.DTOs.User;

public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateDtoValidator()
    {
        RuleFor(u => u.FirstName)
            .NotEmpty()
            .Unless(u => u.FirstName == null)
            .MaximumLength(TextLengthRules.Name);

        RuleFor(u => u.LastName)
            .NotEmpty()
            .Unless(u => u.LastName == null)
            .MaximumLength(TextLengthRules.Name);

        RuleFor(u => u.PhoneNumber)
            .NotEmpty()
            .Unless(u => u.PhoneNumber == null)
            .MaximumLength(TextLengthRules.PhoneNumber);

        RuleFor(u => u.PasswordUpdate)
            .SetValidator(new PasswordUpdateValidator()!)
            .When(u => u.PasswordUpdate != null);

        RuleFor(u => u.BirthDate)
            .ParseableToDate()
            .Unless(u => u.BirthDate == null);
    }
}

public class PasswordUpdateValidator : AbstractValidator<PasswordUpdate>
{
    public PasswordUpdateValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty()
            .WithMessage("Old password is required");

        RuleFor(u => u.NewPassword)
            .MinimumLength(TextLengthRules.PasswordMinLenght)
            .NotEmpty();

        RuleFor(u => u.PasswordConfirm)
            .NotEmpty()
            .Equal(u => u.NewPassword)
            .WithMessage("New password and password confirm do not match.");
    }
}