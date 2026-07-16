using System.Net;
using System.Text;
using ECommerce.Dashboard.DTOs.Order;
using ECommerce.Dashboard.DTOs.Shared;
using ECommerce.Dashboard.Results;

namespace ECommerce.Dashboard.Services.Api;

public class OrderService(ApiRequestService apiRequestService)
{
    private const string OrdersPath = "api/orders";

    public async Task<PaginatedResponse<OrderResponse>> GetOrders(PaginationQuery? paginationQuery = null)
    {
        var query = new StringBuilder("?");
        if (paginationQuery != null)
            query.Append($"limit={paginationQuery.Limit}&page={paginationQuery.Page}");

        var options = new ApiRequestOptions
        {
            Path = OrdersPath + query,
            Method = HttpMethod.Get,
        };

        return await apiRequestService.SendAlwaysSucceedAsync<PaginatedResponse<OrderResponse>>(options);
    }

    public async Task<Result<OrderResponse>> GetOrderById(int id)
    {
        var options = new ApiRequestOptions
        {
            Path = OrdersPath + "/" + id,
            Method = HttpMethod.Get,
        };

        return await apiRequestService.SendAsync<OrderResponse>(options);
    }

    public async Task<PaginatedResponse<OrderResponse>> GetOrdersByClient(
        int clientId,
        PaginationQuery? paginationQuery = null)
    {
        var query = new StringBuilder("?");
        if (paginationQuery != null)
            query.Append($"limit={paginationQuery.Limit}&page={paginationQuery.Page}");

        var options = new ApiRequestOptions
        {
            Path = OrdersPath + "/clients/" + clientId + query,
            Method = HttpMethod.Get,
        };

        return await apiRequestService.SendAlwaysSucceedAsync<PaginatedResponse<OrderResponse>>(options);
    }

    public async Task<PaginatedResponse<OrderResponse>> GetOrdersByProduct(
        int productId,
        PaginationQuery? paginationQuery = null)
    {
        var query = new StringBuilder("?");
        if (paginationQuery != null)
            query.Append($"limit={paginationQuery.Limit}&page={paginationQuery.Page}");

        var options = new ApiRequestOptions
        {
            Path = OrdersPath + "/products/" + productId + query,
            Method = HttpMethod.Get,
        };

        return await apiRequestService.SendAlwaysSucceedAsync<PaginatedResponse<OrderResponse>>(options);
    }
}