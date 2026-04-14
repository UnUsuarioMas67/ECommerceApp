using System.Text.Json.Serialization;
using ECommerce.Api.DTOs.Shared;
using ECommerce.Api.DTOs.User;

namespace ECommerce.Api.DTOs.Cart;

public class CartListResponseDto
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public UserResponseDto? Client { get; set; }
    
    public int Total => Carts.Count();

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public PaginationInfo? Pagination { get; set; }

    public required IEnumerable<CartResponseDto> Carts { get; set; }
}