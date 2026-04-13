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

    public static string GetStatusName(int statusId)
    {
        return statusId switch
        {
            Pending => "PENDING",
            Paid => "PAID",
            Expired => "EXPIRED",
            _ => throw new ArgumentOutOfRangeException(nameof(statusId), statusId, null)
        };
    }

    public static IEnumerable<OrderStatus> GetAll()
        =>
        [
            new() { Id = Pending, Status = GetStatusName(Pending)},
            new() { Id = Paid, Status = GetStatusName(Paid)},
            new() { Id = Expired, Status = GetStatusName(Expired)},
        ];
}