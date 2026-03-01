using ECommerce.Api.Endpoints;

namespace ECommerce.Api.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void MapApiEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGroup("api/login")
            .MapAuthEndpoints();
        endpoints.MapGroup("api/clients")
            .MapClientsEndpoints();
        endpoints.MapGroup("api/addresses")
            .MapAddressEndpoints();
        endpoints.MapGroup("api/categories")
            .MapCategoryEndpoints();
        endpoints.MapGroup("api/products")
            .MapProductEndpoints();
    }
}