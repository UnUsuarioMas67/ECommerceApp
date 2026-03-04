using System.Text.Json.Serialization;
using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Application.DTOs.User;

namespace ECommerce.Api.Application.DTOs.Address;

public class AddressListResponseDto
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Country { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public UserResponseDto? Client { get; set; }

    public int Total => Addresses.Count();
    public PaginationInfo? Pagination { get; set; }
    public required IEnumerable<AddressResponseDto> Addresses { get; set; }
}