using ECommerce.Api.DTOs.Auth;
using ECommerce.Api.DTOs.User;
using ECommerce.Api.Errors;
using ECommerce.Api.Services.Auth;
using ECommerce.Api.Services.DataAccess;
using ECommerce.Api.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var clientGroup = endpoints.MapGroup("api/clients")
            .AllowAnonymous()
            .WithTags("Client");

        clientGroup.MapPost("register", RegisterClient)
            .WithSummary("Register Client");
        clientGroup.MapPost("login", LoginClient)
            .WithSummary("Login Client");
        clientGroup.MapPost("refresh", RefreshClient)
            .WithSummary("Refresh Client login");
        clientGroup.MapPost("logout", LogoutClient)
            .WithSummary("Logout authenticated Client")
            .RequireAuthorization(UserRoles.Client);

        var adminGroup = endpoints.MapGroup("api/admins")
            .AllowAnonymous()
            .WithTags("Admin");

        adminGroup.MapPost("register", RegisterAdmin)
            .WithSummary("Register Admin")
            .RequireAuthorization(UserRoles.Admin);
        adminGroup.MapPost("login", LoginAdmin)
            .WithSummary("Login Admin");
        adminGroup.MapPost("refresh", RefreshAdmin)
            .WithSummary("Refresh Admin login");
        adminGroup.MapPost("logout", LogoutAdmin)
            .WithSummary("Logout authenticated Admin")
            .RequireAuthorization(UserRoles.Admin);

        return endpoints;
    }

    private static async Task<Results<Ok<AuthenticationDto>, UnauthorizedHttpResult, ValidationProblem>> LoginClient(
        AuthenticationService authenticationService,
        LoginRequestDto requestDto,
        IValidator<LoginRequestDto> validator)
    {
        var validation = await validator.ValidateAsync(requestDto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await authenticationService.LoginClient(requestDto.Email, requestDto.Password);

        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.Unauthorized();
    }

    private static async Task<Results<Ok<AuthenticationDto>, UnauthorizedHttpResult, ValidationProblem>> LoginAdmin(
        AuthenticationService authenticationService,
        LoginRequestDto requestDto,
        IValidator<LoginRequestDto> validator)
    {
        var validation = await validator.ValidateAsync(requestDto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await authenticationService.LoginAdmin(requestDto.Email, requestDto.Password);

        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.Unauthorized();
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

    private static async Task<Results<Ok<AuthenticationDto>, UnauthorizedHttpResult, ValidationProblem>> RefreshClient(
        AuthenticationService authenticationService,
        RefreshRequestDto dto,
        IValidator<RefreshRequestDto> validator)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await authenticationService.RefreshClientToken(dto.RefreshToken);
        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.Unauthorized();
    }
    
    private static async Task<Results<Ok<AuthenticationDto>, UnauthorizedHttpResult, ValidationProblem>> RefreshAdmin(
        AuthenticationService authenticationService,
        RefreshRequestDto dto,
        IValidator<RefreshRequestDto> validator)
    {
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var result = await authenticationService.RefreshAdminToken(dto.RefreshToken);
        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.Unauthorized();
    }

    private static async Task<Results<NoContent, UnauthorizedHttpResult>> LogoutClient(
        HttpContext httpContext,
        AuthenticationService authenticationService
    )
    {
        var clientId = AuthUser.GetAuthUserId(httpContext);
        if (!clientId.HasValue)
            return TypedResults.Unauthorized();

        await authenticationService.LogoutClient(clientId.Value);
        return TypedResults.NoContent();
    }
    
    private static async Task<Results<NoContent, UnauthorizedHttpResult>> LogoutAdmin(
        HttpContext httpContext,
        AuthenticationService authenticationService
    )
    {
        var adminId = AuthUser.GetAuthUserId(httpContext);
        if (!adminId.HasValue)
            return TypedResults.Unauthorized();

        await authenticationService.LogoutAdmin(adminId.Value);
        return TypedResults.NoContent();
    }
}