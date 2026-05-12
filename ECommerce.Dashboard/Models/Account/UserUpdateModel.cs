using System.ComponentModel.DataAnnotations;

namespace ECommerce.Dashboard.Models.Account;

public class UserUpdateModel
{
    public int Id { get; set; }
    public string Email { get; set; }
    
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;
    
    [Required]
    [DataType(DataType.PhoneNumber)]
    [StringLength(25)]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Required]
    [DataType(DataType.Date)]
    public DateOnly BirthDate { get; set; }
}