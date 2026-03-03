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

public record PaginationInfo(int Skip, int? Limit)
{
    public static implicit operator PaginationInfo(PaginationQuery query)
        => new(query.Skip ?? PaginationDefaults.Skip, query.Limit);
}