using System.ComponentModel.DataAnnotations;

namespace ECommerce.Dashboard.DTOs.Auth;

public class UserLoginRequest
{
    [EmailAddress]
    [DataType(DataType.EmailAddress)]
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}