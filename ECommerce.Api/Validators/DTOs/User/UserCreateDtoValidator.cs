using ECommerce.Api.DTOs.User;
using ECommerce.Api.Extensions;
using ECommerce.Api.Shared;
using FluentValidation;

namespace ECommerce.Api.Validators.DTOs.User;

public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateDtoValidator()
    {
        RuleFor(u => u.FirstName)
            .NotEmpty()
            .MaximumLength(TextLengthRules.Name);
        
        RuleFor(u => u.LastName)
            .NotEmpty()
            .MaximumLength(TextLengthRules.Name);
        
        RuleFor(u => u.PhoneNumber)
            .NotEmpty()
            .MaximumLength(TextLengthRules.PhoneNumber);
        
        RuleFor(u => u.Password)
            .NotEmpty()
            .MinimumLength(TextLengthRules.PasswordMinLenght);
        
        RuleFor(u => u.PasswordConfirm)
            .NotEmpty()
            .Equal(u => u.Password)
            .WithMessage("Password and confirmation password do not match.");

        RuleFor(u => u.Email)
            .EmailAddress()
            .MaximumLength(TextLengthRules.Email);

        RuleFor(u => u.BirthDate)
            .NotNull()
            .ParseableToDate();
    }
}