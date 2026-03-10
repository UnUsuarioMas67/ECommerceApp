namespace ECommerce.Api.Domain.Entities;

public class Client : IUser
{
    public int Id { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string PhoneNumber { get; set; }
    public DateOnly BirthDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<ShopOrder> Orders { get; set; } = new List<ShopOrder>();
    public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<ClientReview> Reviews { get; set; } = new List<ClientReview>();
}