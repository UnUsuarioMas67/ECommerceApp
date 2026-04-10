using System.Text.Json.Serialization;
using ECommerce.Api.Application.DTOs.Shared;

namespace ECommerce.Api.Application.DTOs.Category;

public class CategoryListResponseDto
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? SearchTerm { get; set; }

    public int Total => Categories.Count();

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public PaginationInfo? Pagination { get; set; }

    public required IEnumerable<CategoryResponseDto> Categories { get; set; }
}