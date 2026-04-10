namespace ECommerce.Api.Entities;

public class OrderLine
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public ShopOrder Order { get; set; }
    public Product Product { get; set; }

    public ICollection<ClientReview> Reviews { get; set; } = new List<ClientReview>();
}