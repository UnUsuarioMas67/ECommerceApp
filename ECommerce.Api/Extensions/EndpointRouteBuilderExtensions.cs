using ECommerce.Api.Application.Endpoints;

namespace ECommerce.Api.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void MapApiEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapAuthEndpoints();
    }
}