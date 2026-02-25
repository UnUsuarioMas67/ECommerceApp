using ECommerce.Api.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Extensions;

public static class RuleBuilderOptionsExtensions
{
    public static IRuleBuilderOptions<TUser, string> EmailNotAlreadyExists<TUser>(
        this IRuleBuilder<TUser, string> ruleBuilder,
        DbSet<TUser> dbSet) where TUser : class, IUser
    {
        return ruleBuilder
            .MustAsync(async (user, email, token) => !await dbSet.AnyAsync(
                u => u.Email == email && u.Id != user.Id))
            .WithMessage("Email address already in use");
    }

    public static IRuleBuilderOptions<TUser, string> PhoneNumberNotAlreadyExists<TUser>(
        this IRuleBuilder<TUser, string> ruleBuilder,
        DbSet<TUser> dbSet) where TUser : class, IUser
    {
        return ruleBuilder
            .MustAsync(async (user, phoneNumber, token)
                => !await dbSet.AnyAsync(
                    u => u.PhoneNumber == phoneNumber && u.Id != user.Id))
            .WithMessage("Phone number already in use");
    }

    public static IRuleBuilderOptions<T, DateTime> NotInTheFuture<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
        where T : class
    {
        return ruleBuilder
            .Must((date) => date <= DateTime.UtcNow)
            .WithMessage("Must not be in the future");
    }

    public static IRuleBuilderOptions<T, DateOnly> NotInTheFuture<T>(this IRuleBuilder<T, DateOnly> ruleBuilder)
        where T : class
    {
        return ruleBuilder
            .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Must not be in the future");
    }

    public static IRuleBuilderOptions<T, string?> CanParseIntoDate<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must(date => DateOnly.TryParse(date, out _))
            .WithMessage("Not a valid date");
    }

    public static IRuleBuilderOptions<T, TProperty?> ExistsInDatabase<T, TProperty>(
        this IRuleBuilder<T, TProperty?> ruleBuilder,
        DbContext dbContext) where TProperty : class
    {
        var dbSet = dbContext.Set<TProperty>();
        return ruleBuilder
            .NotNull()
            .MustAsync(async (entity, token) => await dbSet.AnyAsync(e => e == entity))
            .WithMessage($"The specified {nameof(TProperty)} does not exist.");
    }
}