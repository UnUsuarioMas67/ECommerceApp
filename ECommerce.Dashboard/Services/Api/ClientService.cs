using System.Net;
using System.Text;
using ECommerce.Dashboard.DTOs.Shared;
using ECommerce.Dashboard.DTOs.User;
using ECommerce.Dashboard.Results;

namespace ECommerce.Dashboard.Services.Api;

public class ClientService(ApiRequestService apiRequestService)
{
    private const string ClientsPath = "api/clients";

    public async Task<Result<ClientResponse>> GetClientById(int id)
    {
        var options = new ApiRequestOptions
        {
            Path = ClientsPath + "/" + id,
            Method = HttpMethod.Get,
        };

        return await apiRequestService.SendAsync<ClientResponse>(options);
    }

    public async Task<IEnumerable<ClientResponse>> GetClients(
        string? search = null,
        PaginationQuery? paginationQuery = null)
    {
        var query = new StringBuilder($"?search={search ?? ""}");
        if (paginationQuery != null)
            query.Append($"&limit={paginationQuery.Limit}&page={paginationQuery.Page}");

        var options = new ApiRequestOptions
        {
            Path = ClientsPath + query,
            Method = HttpMethod.Get,
        };

        return await apiRequestService.SendAlwaysSucceedAsync<IEnumerable<ClientResponse>>(options);
    }
}