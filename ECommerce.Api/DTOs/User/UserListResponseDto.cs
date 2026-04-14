using System.Text.Json.Serialization;
using ECommerce.Api.DTOs.Shared;

namespace ECommerce.Api.DTOs.User;

public class UserListResponseDto
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? SearchTerm { get; set; }

    public int Total => Users.Count();
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public PaginationInfo? Pagination { get; set; }
    public required IEnumerable<UserResponseDto> Users { get; set; }
}