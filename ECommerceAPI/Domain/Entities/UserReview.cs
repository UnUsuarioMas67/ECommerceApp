namespace ECommerceAPI.Domain.Entities;

public class UserReview
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int OrderLineId { get; set; }
    
    public string Comment { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public User User { get; set; }
    public OrderLine OrderLine { get; set; }
}