using System.Text.Json.Serialization;
using ECommerce.Api.Application.DTOs.Shared;
using ECommerce.Api.Application.DTOs.User;

namespace ECommerce.Api.Application.DTOs.Cart;

public class CartListResponseDto
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public UserResponseDto? Client { get; set; }
    
    public int Total => Carts.Count();

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public PaginationInfo? Pagination { get; set; }

    public required IEnumerable<CartResponseDto> Carts { get; set; }
}