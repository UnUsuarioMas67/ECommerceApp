using ECommerce.Api.Application.DTOs.User;
using ECommerce.Api.Domain.Validation;
using ECommerce.Api.Extensions;
using FluentValidation;

namespace ECommerce.Api.Validators.DTOs;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(u => u.Password)
            .NotEmpty()
            .MinimumLength(TextLengthRules.PasswordMinLenght);
        
        RuleFor(u => u.FirstName)
            .NotEmpty()
            .MaximumLength(TextLengthRules.Name);

        RuleFor(u => u.LastName)
            .NotEmpty()
            .MaximumLength(TextLengthRules.Name);

        RuleFor(u => u.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(TextLengthRules.Email);

        RuleFor(u => u.PhoneNumber)
            .NotEmpty()
            .MaximumLength(TextLengthRules.PhoneNumber);

        RuleFor(u => u.BirthDate)
            .NotNull()
            .CanParseIntoDate();
    }
}