using ECommerce.Api.Application.Auth;
using ECommerce.Api.Application.DTOs.Auth;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ECommerce.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("api/login/client", LoginClient);
        endpoints.MapPost("api/login/admin", LoginAdmin);
        
        return endpoints;
    }
    
    private static async Task<Ok<AuthenticationDto>> LoginClient(
        IAuthenticationService authenticationService,
        LoginRequestDto requestDto)
    {
        var result = await authenticationService.Login<Client>(requestDto.Email, requestDto.Password);
        return TypedResults.Ok(result.ToDto());
    }
    
    private static async Task<Ok<AuthenticationDto>> LoginAdmin(
        IAuthenticationService authenticationService, 
        LoginRequestDto requestDto)
    {
        var result = await authenticationService.Login<Admin>(requestDto.Email, requestDto.Password);
        return TypedResults.Ok(result.ToDto());
    }
}