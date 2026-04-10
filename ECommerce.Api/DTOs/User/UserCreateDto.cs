namespace ECommerce.Api.Application.DTOs.User;

// ReSharper disable once ClassNeverInstantiated.Global
public class UserCreateDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public string BirthDate { get; set; }
}