using ECommerce.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.EF.Seeding;

public class AdminSeed
{
    public void AddData(DbContext context)
    {
        var adminSet = context.Set<Admin>();
        var hasAny = adminSet.Any();
        if (hasAny)
            return;

        var admin = new Admin
        {
            FirstName = "Bryan",
            LastName = "Mendoza",
            Email = "bm123@email.com",
            PhoneNumber = "+16667778888",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345678"),
            BirthDate = new DateOnly(2001, 7, 22),
            CreatedAt = DateTime.Parse("2026-01-26"),
        };
        adminSet.Add(admin);
        context.SaveChanges();
    }

    public async Task AddDataAsync(DbContext context, CancellationToken ct)
    {
        var adminSet = context.Set<Admin>();
        var hasAny = await adminSet.AnyAsync(ct);
        if (hasAny)
            return;

        var admin = new Admin
        {
            FirstName = "Bryan",
            LastName = "Mendoza",
            Email = "bm123@email.com",
            PhoneNumber = "+16667778888",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345678"),
            BirthDate = new DateOnly(2001, 7, 22),
            CreatedAt = DateTime.Parse("2026-01-26"),
        };
        await adminSet.AddAsync(admin, ct);
        await context.SaveChangesAsync(ct);
    }
}