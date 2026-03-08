using ECommerce.Api.Application.DTOs.Auth;
using ECommerce.Api.Application.Services.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ECommerce.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("client", LoginClient);
        endpoints.MapPost("admin", LoginAdmin);

        return endpoints;
    }

    // private static async Task<Results<Ok<AuthenticationDto>, ValidationProblem>> Login<T>(
    //     IAuthenticationService authenticationService,
    //     LoginRequestDto requestDto,
    //     IValidator<LoginRequestDto> validator) where T : class, IUser
    // {
    //     var validation = await validator.ValidateAsync(requestDto);
    //     if (!validation.IsValid)
    //         return TypedResults.ValidationProblem(validation.ToDictionary());
    //     
    //     var result = await authenticationService.Login<T>(requestDto.Email, requestDto.Password);
    //     return TypedResults.Ok(result.GetDto());
    // }
    
    private static async Task<Results<Ok<AuthenticationDto>, Ok<string>, ValidationProblem>> LoginClient(
        IAuthenticationService authenticationService,
        LoginRequestDto requestDto,
        IValidator<LoginRequestDto> validator)
    {
        var validation = await validator.ValidateAsync(requestDto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());
        
        var result = await authenticationService.LoginClient(requestDto.Email, requestDto.Password);
        
        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.Ok("Invalid credentials");
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