using ECommerce.Dashboard.DTOs.Order;

namespace ECommerce.Dashboard.Models.Orders;

public class OrderListViewModel
{
    public IEnumerable<OrderResponse> Orders { get; set; } = [];
    public string? Status { get; set; }
    public required int Page { get; set; }
    public required int TotalPages { get; set; }
}