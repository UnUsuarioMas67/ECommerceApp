namespace ECommerceAPI.Data.Entities;

public class User
{
    public int Id { get; set; }
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string PhoneNumber { get; set; }
    public DateOnly BirthDate { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public ICollection<ShopOrder> Orders { get; set; }
    public ICollection<Cart> Carts { get; set; }
    public ICollection<Address> Addresses { get; set; }
    public ICollection<UserReview> Reviews { get; set; }
}