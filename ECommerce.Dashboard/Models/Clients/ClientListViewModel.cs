using ECommerce.Dashboard.DTOs.User;

namespace ECommerce.Dashboard.Models.Clients;

public class ClientListViewModel
{
    public IEnumerable<ClientResponse> Clients { get; set; } = [];
    public string? SearchTerm { get; set; }
}