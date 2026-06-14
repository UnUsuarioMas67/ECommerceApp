using ECommerce.Api.Entities;
using Microsoft.EntityFrameworkCore;
using RESTCountries.NET.Services;

namespace ECommerce.Api.EF.Seeding;

public class CountrySeed
{
    public void AddData(DbContext context)
    {
        var countries = context.Set<Country>();
        if (!countries.Any())
        {
            var countriesToAdd = RestCountriesService.GetAllCountries()
                .Select(country => new Country { Name = country.Name.Common, Cca2 = country.Cca2 });

            countries.AddRange(countriesToAdd);
        }

        context.SaveChanges();
    }

    public async Task AddDataAsync(DbContext context, CancellationToken ct)
    {
        var countries = context.Set<Country>();
        if (!await countries.AnyAsync(ct))
        {
            var countriesToAdd = RestCountriesService.GetAllCountries()
                .Select(country => new Country { Name = country.Name.Common, Cca2 = country.Cca2 });

            await countries.AddRangeAsync(countriesToAdd, ct);
        }

        await context.SaveChangesAsync(ct);
    }
}