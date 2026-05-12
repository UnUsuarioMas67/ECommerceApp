namespace ECommerce.Dashboard.DTOs.User;

public class AdminUserUpdate
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PhoneNumber { get; set; }
    public required string BirthDate { get; set; }
}