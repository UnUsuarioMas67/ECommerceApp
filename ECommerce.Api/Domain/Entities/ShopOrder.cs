namespace ECommerce.Api.Domain.Entities;

public class ShopOrder
{
    public int Id { get; set; }
    public int? ClientId { get; set; }
    public int AddressId { get; set; }
    public int StatusId { get; set; }
    
    public DateTime OrderDate { get; set; }
    
    public Client? Client { get; set; }
    public Address Address { get; set; }
    public OrderStatus Status { get; set; }
    
    public ICollection<OrderLine> Items { get; set; } = new List<OrderLine>();
}