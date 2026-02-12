using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Domain.Validation;
using ECommerce.Api.Extensions;
using ECommerce.Api.Infrastructure.EF;
using FluentValidation;

namespace ECommerce.Api.Application.Validators;

public class UserValidator<T> : AbstractValidator<IUser> where T : class, IUser
{
    public UserValidator(ECommerceContext db)
    {
        var users = db.Set<T>();
        
        RuleFor(u => u.FirstName)
            .NotEmpty()
            .MaximumLength(TextLengthRules.Name);

        RuleFor(u => u.LastName)
            .NotEmpty()
            .MaximumLength(TextLengthRules.Name);

        RuleFor(u => u.Email)
            .NotEmpty()
            .MaximumLength(TextLengthRules.Email);

        RuleFor(u => u.PhoneNumber)
            .NotEmpty()
            .MaximumLength(TextLengthRules.PhoneNumber);

        RuleFor(u => u.BirthDate)
            .NotNull()
            .NotInTheFuture();

        RuleFor(u => u.CreatedAt)
            .NotNull()
            .NotInTheFuture();

        RuleFor(u => u.PasswordHash)
            .NotEmpty();

        RuleFor(u => u.Email)
            .NotEmpty()
            .MaximumLength(TextLengthRules.Email)
            .EmailNotAlreadyExists(users);

        RuleFor(u => u.PhoneNumber)
            .NotEmpty()
            .MaximumLength(TextLengthRules.PhoneNumber)
            .PhoneNumberNotAlreadyExists(users);
    }
}