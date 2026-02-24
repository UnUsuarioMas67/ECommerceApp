using ECommerce.Api.Application.Auth;
using ECommerce.Api.Application.DTOs.Auth;
using ECommerce.Api.Domain.Entities;
using ECommerce.Api.Extensions;
using ECommerce.Api.Extensions.Mappings;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ECommerce.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("api/login/client", Login<Client>);
        endpoints.MapPost("api/login/admin", Login<Admin>);

        return endpoints;
    }

    private static async Task<Results<Ok<AuthenticationDto>, ValidationProblem>> Login<T>(
        IAuthenticationService authenticationService,
        LoginRequestDto requestDto,
        IValidator<LoginRequestDto> validator) where T : class, IUser
    {
        var validation = await validator.ValidateAsync(requestDto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());
        
        var result = await authenticationService.Login<T>(requestDto.Email, requestDto.Password);
        return TypedResults.Ok(result.GetDto());
    }
}