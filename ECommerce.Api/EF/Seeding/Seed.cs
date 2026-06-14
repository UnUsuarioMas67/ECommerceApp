using System.Text.Json;
using ECommerce.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.EF.Seeding;

public class Seed<T>(string filePath)
    where T : class
{
    protected virtual T[] LoadFromJson()
    {
        var lines = File.ReadAllText(filePath);
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        var records = JsonSerializer.Deserialize<T[]>(lines, options);
        if (records == null)
            throw new Exception("No client records found");
        
        return records;
    }

    public virtual void AddData(DbContext context)
    {
        var set = context.Set<T>();
        var hasAny = set.Any();
        if (hasAny)
            return;

        var records = LoadFromJson();
        if (records.Length == 0)
            throw new Exception("No records found");

        set.AddRange(records);
        context.SaveChanges();
    }

    public virtual async Task AddDataAsync(DbContext context, CancellationToken ct)
    {
        var set = context.Set<T>();
        var hasAny = await set.AnyAsync(ct);
        if (hasAny)
            return;

        var records = LoadFromJson();
        if (records.Length == 0)
            throw new Exception("No records found");

        await set.AddRangeAsync(records, ct);
        await context.SaveChangesAsync(ct);
    }
}