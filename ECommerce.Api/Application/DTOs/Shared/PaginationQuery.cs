using System.Text.Json.Serialization;

namespace ECommerce.Api.Application.DTOs.Shared;

public class PaginationQuery
{
    public int? Limit { get; set; }
    public int? Skip { get; set; }
}

public static class PaginationDefaults
{
    public const int Skip = 0;
    public const int Limit = 100;
}

public record PaginationInfo(
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] int? Skip, 
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] int? Limit)
{
    public static implicit operator PaginationInfo(PaginationQuery query)
        => new(query.Skip, query.Limit);
}