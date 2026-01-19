namespace ECommerceAPI.Domain.Entities;

public class ShopOrder
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AddressId { get; set; }
    public int StatusId { get; set; }
    
    public DateTime OrderDate { get; set; }
    
    public User User { get; set; }
    public Address Address { get; set; }
    public OrderStatus Status { get; set; }
    
    public ICollection<OrderLine> Items { get; set; }
}