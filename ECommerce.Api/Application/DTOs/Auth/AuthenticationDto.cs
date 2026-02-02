using System.Text.Json.Serialization;
using ECommerce.Api.Application.DTOs.User;

namespace ECommerce.Api.Application.DTOs.Auth;

public class AuthenticationDto
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Message { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public UserResponseDto? User { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Token { get; set; }
}