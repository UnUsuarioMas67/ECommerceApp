namespace ECommerce.Api.Application.DTOs.Shared;

public class PaginationQuery
{
    public int? Limit { get; set; }
    public int? Skip { get; set; }
}