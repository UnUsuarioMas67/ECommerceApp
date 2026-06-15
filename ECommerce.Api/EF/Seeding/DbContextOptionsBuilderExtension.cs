using ECommerce.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.EF.Seeding;

public static class DbContextOptionsBuilderExtension
{
    public static void AddSeedingToDbContext(this DbContextOptionsBuilder optionsBuilder)
    {
        var categorySeed = new Seed<Category>(@"EF\Seeding\InitialData\categories.json");
        var clientSeed = new ClientSeed(@"EF\Seeding\InitialData\clients.json");
        var productSeed = new Seed<Product>(@"EF\Seeding\InitialData\products.json");
        var countrySeed = new CountrySeed();
        var addressSeed = new Seed<Address>(@"EF\Seeding\InitialData\addresses.json");
        var adminSeed = new Seed<Admin>(@"EF\Seeding\InitialData\admins.json");
        optionsBuilder.UseSeeding((context, _) =>
        {
            categorySeed.AddData(context);
            clientSeed.AddData(context);
            productSeed.AddData(context);
            countrySeed.AddData(context);
            addressSeed.AddData(context);
            adminSeed.AddData(context);
        }).UseAsyncSeeding(async (context, _, ct) =>
        {
            await categorySeed.AddDataAsync(context, ct);
            await clientSeed.AddDataAsync(context, ct);
            await productSeed.AddDataAsync(context, ct);
            await countrySeed.AddDataAsync(context, ct);
            await addressSeed.AddDataAsync(context, ct);
            await adminSeed.AddDataAsync(context,ct);
        });
    }
}