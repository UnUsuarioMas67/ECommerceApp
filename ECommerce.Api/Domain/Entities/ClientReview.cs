namespace ECommerce.Api.Domain.Entities;

public class ClientReview
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int OrderLineId { get; set; }
    
    public string Comment { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public Client Client { get; set; }
    public OrderLine OrderLine { get; set; }
}