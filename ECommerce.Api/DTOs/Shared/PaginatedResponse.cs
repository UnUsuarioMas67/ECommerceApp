namespace ECommerce.Api.DTOs.Shared;

public class PaginatedResponse<T>
{
    public required int Limit { get; set; }
    public required int Page { get; set; }
    public required int TotalPages { get; set; }
    public required List<T> Items { get; set; }
}