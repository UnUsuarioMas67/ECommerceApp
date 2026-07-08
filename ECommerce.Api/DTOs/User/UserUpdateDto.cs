namespace ECommerce.Api.DTOs.User;

// ReSharper disable once ClassNeverInstantiated.Global
public class UserUpdateDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public PasswordUpdate? PasswordUpdate { get; set; }
    public string? PhoneNumber { get; set; }
    public string? BirthDate { get; set; }
}

public class PasswordUpdate
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string PasswordConfirm { get; set; } = string.Empty;
}