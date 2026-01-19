namespace ECommerceAPI.Domain.Entities;

public class OrderLine
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    
    public ShopOrder Order { get; set; }
    public Product Product { get; set; }
    
    public ICollection<UserReview> Reviews { get; set; }
}