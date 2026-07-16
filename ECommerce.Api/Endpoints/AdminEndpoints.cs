using ECommerce.Api.DTOs.Shared;
using ECommerce.Api.DTOs.Error;
using ECommerce.Api.DTOs.User;
using ECommerce.Api.Errors;
using ECommerce.Api.Services.DataAccess;
using ECommerce.Api.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Endpoints;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("api/admins")
            .WithTags("Admin")
            .RequireAuthorization(UserRoles.Admin);

        group.MapGet("", GetAdmins)
            .WithSummary("Get all admins");
        group.MapGet("me", GetAuthAdmin)
            .WithSummary("Get authenticated admin");
        group.MapGet("{id:int}", GetAdminById)
            .WithSummary("Get admin by id");
        group.MapPut("", UpdateAdmin)
            .WithSummary("Update admin");
        group.MapDelete("", DeleteAdmin)
            .WithSummary("Delete admin");

        return endpoints;
    }

    private static async Task<Ok<PaginatedResponse<UserResponseDto>>> GetAdmins(
        IAdminsService adminsService,
        [AsParameters] PaginationQuery pagination,
        [FromQuery] string? search = null)
    {
        var admins = await adminsService.GetManyAsync(pagination, search);
        return TypedResults.Ok(admins);
    }

    private static async Task<Results<Ok<UserResponseDto>, UnauthorizedHttpResult>> GetAuthAdmin(
        HttpContext httpContext,
        IAdminsService adminsService)
    {
        var adminId = AuthUser.GetAuthUserId(httpContext);
        if (adminId == null)
            return TypedResults.Unauthorized();

        var admin = await adminsService.GetByIdAsync(adminId.Value);
        return admin != null 
            ? TypedResults.Ok(admin) 
            : TypedResults.Unauthorized();
    }

    private static async Task<Results<Ok<UserResponseDto>, NotFound>> GetAdminById(
        IAdminsService adminsService,
        int id)
    {
        var admin = await adminsService.GetByIdAsync(id);
        return admin != null ? TypedResults.Ok(admin) : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<UserResponseDto>, BadRequest<ErrorDto>,
            UnauthorizedHttpResult, UnprocessableEntity<ErrorDto>>>
        UpdateAdmin(
            HttpContext context,
            IAdminsService adminsService,
            UserUpdateDto dto,
            IValidator<UserUpdateDto> validator)
    {
        var adminId = AuthUser.GetAuthUserId(context);
        if (adminId == null)
            return TypedResults.Unauthorized();

        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return TypedResults.BadRequest(new ValidationError(validation.ToDictionary()).ToDto());

        var result = await adminsService.UpdateAsync(adminId.Value, dto);
        if (result.IsSuccess)
            return TypedResults.Ok(result.Value);

        return result.Error switch
        {
            NotFoundError => TypedResults.Unauthorized(),
            ValidationError error => TypedResults.BadRequest(error.ToDto()),
            _ => TypedResults.UnprocessableEntity(result.Error.ToDto())
        };
    }

    private static async Task<Results<Ok<UserResponseDto>, UnauthorizedHttpResult>> DeleteAdmin(
        HttpContext context,
        IAdminsService adminsService)
    {
        var adminId = AuthUser.GetAuthUserId(context);
        if (adminId == null)
            return TypedResults.Unauthorized();

        var admin = await adminsService.DeleteAsync(adminId.Value);
        return admin != null 
            ? TypedResults.Ok(admin) 
            : TypedResults.Unauthorized();
    }
}