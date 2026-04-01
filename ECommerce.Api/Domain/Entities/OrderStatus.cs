namespace ECommerce.Api.Domain.Entities;

public class OrderStatus
{
    // TODO - Add predefined statuses
    public int Id { get; set; }
    public string Status { get; set; }
}

public static class OrderStatuses
{
    public const int Pending = 1;
    public const int Success = 2;
    public const int Failed = 3;

    public static IEnumerable<OrderStatus> GetAll()
        =>
        [
            new() { Id = Pending, Status = "Pending"},
            new() { Id = Success, Status = "Success"},
            new() { Id = Failed, Status = "Failed"},
        ];
}