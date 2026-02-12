using ECommerce.Api.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Extensions;

public static class RuleBuilderOptionsExtensions
{
    public static IRuleBuilderOptions<IUser, string> EmailNotAlreadyExists<T>(
        this IRuleBuilder<IUser, string> ruleBuilder,
        DbSet<T> dbSet) where T : class, IUser
    {
        return ruleBuilder
            .MustAsync(async (email, token) => !await dbSet.AnyAsync(u => u.Email == email))
            .WithMessage("Email address already in use");
    }
    
    public static IRuleBuilderOptions<IUser, string> PhoneNumberNotAlreadyExists<T>(
        this IRuleBuilder<IUser, string> ruleBuilder,
        DbSet<T> dbSet) where T : class, IUser
    {
        return ruleBuilder
            .MustAsync(async (phoneNumber, token) 
                => !await dbSet.AnyAsync(u => u.PhoneNumber == phoneNumber))
            .WithMessage("Phone number already in use");
    }

    public static IRuleBuilderOptions<T, DateTime> NotInTheFuture<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
        where T : class
    {
        return ruleBuilder
            .Must((date) => date <= DateTime.Today)
            .WithMessage("Must not be in the future");
    }
    
    public static IRuleBuilderOptions<T, DateOnly> NotInTheFuture<T>(this IRuleBuilder<T, DateOnly> ruleBuilder)
        where T : class
    {
        return ruleBuilder
            .Must(date => date <= DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Must not be in the future");
    }
}