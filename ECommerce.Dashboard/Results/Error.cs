namespace ECommerce.Dashboard.Results;

public record Error(string Message)
{
    public static Error None() => new Error("None");
}