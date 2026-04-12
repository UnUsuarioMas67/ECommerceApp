namespace ECommerce.Api.Entities;

public class OrderStatus
{
    // TODO - Add predefined statuses
    public int Id { get; set; }
    public string Status { get; set; }
}

public static class OrderStatuses
{
    public const int Pending = 1;
    public const int Paid = 2;
    public const int Expired = 3;

    public static IEnumerable<OrderStatus> GetAll()
        =>
        [
            new() { Id = Pending, Status = "PENDING"},
            new() { Id = Paid, Status = "PAID"},
            new() { Id = Expired, Status = "EXPIRED"},
        ];
}