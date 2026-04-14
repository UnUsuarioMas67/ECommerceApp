using System.Text.Json.Serialization;
using ECommerce.Api.DTOs.Shared;

namespace ECommerce.Api.DTOs.Product;

public class ProductListResponseDto
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Category { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? SearchTerm { get; set; }

    public int Total => Products.Count();

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public PaginationInfo? Pagination { get; set; }

    public required IEnumerable<ProductResponseDto> Products { get; set; }
}