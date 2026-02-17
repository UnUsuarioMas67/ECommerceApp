using ECommerce.Api.Application.Endpoints;

namespace ECommerce.Api.Extensions;

public static class EndpointRouteBuilderExtension
{
    public static void MapApiEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapAuthEndpoints();
    }
}