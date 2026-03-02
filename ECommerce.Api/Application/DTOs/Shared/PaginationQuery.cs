namespace ECommerce.Api.Application.DTOs.Shared;

public class PaginationQuery
{
    public int? Limit { get; set; }
    public int? Skip { get; set; }
}

public static class PaginationDefaults
{
    public static int Skip = 0;
    public static int Limit = 100;
}