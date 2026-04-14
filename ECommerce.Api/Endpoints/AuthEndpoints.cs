using ECommerce.Api.DTOs.Auth;
using ECommerce.Api.DTOs.User;
using ECommerce.Api.Errors;
using ECommerce.Api.Services.Auth;
using ECommerce.Api.Services.DataAccess;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ECommerce.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var clientGroup = endpoints.MapGroup("api/clients")
            .AllowAnonymous()
            .WithTags("Client");
        
        clientGroup.MapPost("login", LoginClient)
            .WithSummary("Login Client");
        clientGroup.MapPost("register", RegisterClient)
            .WithSummary("Register Client");

        var adminGroup = endpoints.MapGroup("api/admins")
            .AllowAnonymous()
            .WithTags("Admin");
        
        adminGroup.MapPost("login", LoginAdmin)
            .WithSummary("Login Admin");
        adminGroup.MapPost("register", RegisterAdmin)
            .WithSummary("Register Admin");

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

    private static async Task<Results<Created<UserResponseDto>, ValidationProblem, UnprocessableEntity<Error>>> 
        RegisterAdmin(
            HttpContext httpContext,
            IAdminsService adminsService,
            UserCreateDto dto,
            IValidator<UserCreateDto> validator)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await adminsService.CreateAsync(dto);
        if (result.IsSuccess)
        {
            var path = httpContext.Request.Path;
            return TypedResults.Created($"{path}/{result.Value!.Id}", result.Value);
        }

        if (result.Error is ValidationError error)
            return TypedResults.ValidationProblem(error.Details);
        
        return TypedResults.UnprocessableEntity(result.Error);
    }
    
    private static async Task<Results<Created<UserResponseDto>, ValidationProblem, UnprocessableEntity<Error>>> 
        RegisterClient(
            HttpContext httpContext,
            IClientsService clientsService,
            UserCreateDto dto,
            IValidator<UserCreateDto> validator)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await clientsService.CreateAsync(dto);
        if (result.IsSuccess)
        {
            var path = httpContext.Request.Path;
            return TypedResults.Created($"{path}/{result.Value!.Id}", result.Value);
        }

        if (result.Error is ValidationError error)
            return TypedResults.ValidationProblem(error.Details);
        
        return TypedResults.UnprocessableEntity(result.Error);
    }
}