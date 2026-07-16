using ECommerce.Dashboard.DTOs.Client;

namespace ECommerce.Dashboard.Models.Clients;

public class ClientListViewModel
{
    public IEnumerable<ClientResponse> Clients { get; set; } = [];
    public string? SearchTerm { get; set; }
    public required int Page { get; set; }
    public required int TotalPages { get; set; }
}