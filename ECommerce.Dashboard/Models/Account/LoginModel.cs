using System.ComponentModel.DataAnnotations;

namespace ECommerce.Dashboard.Models.Account;

public class LoginModel
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