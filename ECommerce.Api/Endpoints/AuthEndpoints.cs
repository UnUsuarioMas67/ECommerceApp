using ECommerce.Api.Application.DTOs.Auth;
using ECommerce.Api.Application.Services.Auth;
using ECommerce.Api.Shared.Errors;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ECommerce.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/login");
        group.MapPost("client", LoginClient);
        group.MapPost("admin", LoginAdmin);

        return endpoints;
    }
    
    private static async Task<Results<Ok<AuthenticationDto>, Ok<Error>, ValidationProblem>> LoginClient(
        IAuthenticationService authenticationService,
        LoginRequestDto requestDto,
        IValidator<LoginRequestDto> validator)
    {
        var validation = await validator.ValidateAsync(requestDto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());
        
        var result = await authenticationService.LoginClient(requestDto.Email, requestDto.Password);
        
        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.Ok(result.Error);
    }
    
    private static async Task<Results<Ok<AuthenticationDto>, Ok<string>, ValidationProblem>> LoginAdmin(
        IAuthenticationService authenticationService,
        LoginRequestDto requestDto,
        IValidator<LoginRequestDto> validator)
    {
        var validation = await validator.ValidateAsync(requestDto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());
        
        var result = await authenticationService.LoginAdmin(requestDto.Email, requestDto.Password);
        
        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.Ok("Invalid credentials");
    }
}