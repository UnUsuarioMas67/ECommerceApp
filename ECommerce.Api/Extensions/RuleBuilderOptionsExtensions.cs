using ECommerce.Api.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ECommerce.Api.Extensions;

public static class RuleBuilderOptionsExtensions
{
    public static IRuleBuilderOptions<T, int> IdIsDefaultOnNewEntry<T>(
        this IRuleBuilder<T, int> ruleBuilder, Func<T, EntityEntry<T>> predicate) where T : class
    {
        return ruleBuilder
            .Equal(0)
            .When(client => predicate(client).State == EntityState.Detached);
    }
    
    public static IRuleBuilderOptions<T, DateTime> NotInTheFuture<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
        where T : class
    {
        return ruleBuilder
            .Must(date => date <= DateTime.UtcNow)
            .WithMessage("Must not be in the future");
    }

    public static IRuleBuilderOptions<T, DateOnly> NotInTheFuture<T>(this IRuleBuilder<T, DateOnly> ruleBuilder)
        where T : class
    {
        return ruleBuilder
            .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Must not be in the future");
    }

    public static IRuleBuilderOptions<T, string?> ParseableToDate<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Must(date => DateOnly.TryParseExact(date, "yyyy-MM-dd", out _))
            .WithMessage("{PropertyName} must be in valid date format. (yyyy-MM-dd)");
    }
}