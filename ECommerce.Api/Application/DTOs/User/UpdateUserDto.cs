namespace ECommerce.Api.Application.DTOs.User;

public class UpdateUserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Password { get; set; }
    public string? PhoneNumber { get; set; }
    public string? BirthDate { get; set; }
}