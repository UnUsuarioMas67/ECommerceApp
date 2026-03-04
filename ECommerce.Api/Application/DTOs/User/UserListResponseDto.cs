using System.Text.Json.Serialization;
using ECommerce.Api.Application.DTOs.Shared;

namespace ECommerce.Api.Application.DTOs.User;

public class UserListResponseDto
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? SearchTerm { get; set; }

    public int Total => Users.Count();
    public required PaginationInfo Pagination { get; set; }
    public required IEnumerable<UserResponseDto> Users { get; set; }
}