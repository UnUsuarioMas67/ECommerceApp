using System.Text.Json.Serialization;

namespace ECommerce.Api.DTOs.Shared;

public class PaginationQuery
{
    public int? Limit { get; set; }
    public int? Page { get; set; }
    
    public int LimitOrDefault => Limit ?? 20;
    public int PageOrDefault => Page ?? 1;
}

public record PaginationInfo(
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] int Page, 
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] int Limit)
{
    public static implicit operator PaginationInfo(PaginationQuery query)
        => new(query.PageOrDefault, query.LimitOrDefault);
}