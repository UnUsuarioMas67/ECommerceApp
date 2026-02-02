using System.Text.Json.Serialization;

namespace ECommerce.Api.Application.DTOs;

public class AuthenticationDto
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Message { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public UserResponseDto? User { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Token { get; set; }
}